using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;

namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly INoSQLTableStorage<TransactionEntity> _table;


        public TransactionRepository(
            INoSQLTableStorage<TransactionEntity> table)
        {
            _table = table;
        }


        private static string GetPartitionKey(Guid operationId)
        {
            return $"{operationId}";
        }

        private static string GetRowKey(string txData)
        {
            return txData.CalculateHexHash64();
        }

        public async Task AddAsync(BuiltTransactionDto dto)
        {
            var entity = new TransactionEntity
            {
                Amount = dto.Amount.ToString(),
                BuiltOn = dto.BuiltOn,
                FromAddress = dto.FromAddress,
                GasPrice = dto.GasPrice.ToString(),
                IncludeFee = dto.IncludeFee,
                Nonce = dto.Nonce.ToString(),
                OperationId = dto.OperationId,
                State = dto.State.ToString(),
                ToAddress = dto.ToAddress,
                TxData = dto.TxData,

                PartitionKey = GetPartitionKey(dto.OperationId),
                RowKey = GetRowKey(dto.TxData)
            };

            await _table.InsertAsync(entity);
        }

        public async Task<bool> DeleteIfExistsAsync(Guid operationId)
        {
            var entities = (await InnerGetAllAsync(operationId)).ToList();

            if (entities.Any())
            {
                await _table.DeleteAsync(entities);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<TransactionDto>> GetAllAsync(Guid operationId)
        {
            return (await InnerGetAllAsync(operationId))
                .Select(x => x.ToDto());
        }

        public async Task<IEnumerable<TransactionDto>> GetAllInProgressAsync()
        {
            return (await _table.GetDataAsync(x => x.State == TransactionState.InProgress.ToString()))
                .Select(x => x.ToDto());
        }

        public async Task UpdateAsync(BroadcastedTransactionDto dto)
        {
            TransactionEntity UpdateAction(TransactionEntity entity)
            {
                entity.BroadcastedOn = dto.BroacastedOn;
                entity.SignedTxData = dto.SignedTxData;
                entity.SignedTxHash = dto.SignedTxHash;
                entity.State = dto.State.ToString();
                
                return entity;
            }

            await _table.MergeAsync
            (
                GetPartitionKey(dto.OperationId),
                GetRowKey(dto.TxData),
                UpdateAction
            );
        }

        public async Task UpdateAsync(CompletedTransactionDto dto)
        {
            TransactionEntity UpdateAction(TransactionEntity entity)
            {
                entity.CompletedOn = dto.CompletedOn;
                entity.Error = dto.Error;
                entity.State = dto.State.ToString();

                return entity;
            }

            await _table.MergeAsync
            (
                GetPartitionKey(dto.OperationId),
                GetRowKey(dto.TxData),
                UpdateAction
            );
        }

        private async Task<IEnumerable<TransactionEntity>> InnerGetAllAsync(Guid operationId)
        {
            return (await _table.GetDataAsync(GetPartitionKey(operationId)));
        }
    }
}
