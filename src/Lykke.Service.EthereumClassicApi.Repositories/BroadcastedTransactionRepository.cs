using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;


namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class BroadcastedTransactionRepository : IBroadcastedTransactionRepository
    {
        private readonly INoSQLTableStorage<BroadcastedTransactionEntity> _table;


        public BroadcastedTransactionRepository(
            INoSQLTableStorage<BroadcastedTransactionEntity> table)
        {
            _table = table;
        }


        private static string GetPartitionKey(Guid operationId)
        {
            return operationId.ToString();
        }

        private static string GetRowKey(string signedTxData)
        {
            return signedTxData.CalculateHexHash64();
        }


        public async Task AddOrReplaceAsync(BroadcastedTransactionDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.OperationId);
            entity.RowKey = GetRowKey(dto.SignedTxData);

            await _table.InsertOrReplaceAsync(entity);
        }

        public async Task DeleteAsync(Guid operationId)
        {
            var entities = await InnerGetAsync(operationId);

            await _table.DeleteAsync(entities);
        }

        public async Task<IEnumerable<BroadcastedTransactionDto>> GetAllAsync()
        {
            var dtos = new List<BroadcastedTransactionDto>();

            string continuationToken = null;

            do
            {
                IEnumerable<BroadcastedTransactionEntity> entities;

                (entities, continuationToken) = await _table.GetDataWithContinuationTokenAsync(1000, continuationToken);

                dtos.AddRange(entities.Select(x => x.ToDto()));

            } while (continuationToken != null);

            return dtos;
        }

        public async Task<bool> ExistsAsync(Guid operationId, string signedTxData)
        {
            var entity = await _table.GetDataAsync
            (
                GetPartitionKey(operationId),
                GetRowKey(signedTxData)
            );

            return entity != null;
        }

        public async Task<IEnumerable<BroadcastedTransactionDto>> GetAsync(Guid operationId)
        {
            return (await InnerGetAsync(operationId))
                .Select(x => x.ToDto());
        }

        private async Task<IEnumerable<BroadcastedTransactionEntity>> InnerGetAsync(Guid operationId)
        {
            return await _table.GetDataAsync(GetPartitionKey(operationId));
        }
    }
}
