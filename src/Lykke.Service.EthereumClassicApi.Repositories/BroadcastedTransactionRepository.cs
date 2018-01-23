using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class BroadcastedTransactionRepository : IBroadcastedTransactionRepository
    {
        private readonly IAddStrategy<BroadcastedTransactionEntity>    _addStrategy;
        private readonly IDeleteStrategy<BroadcastedTransactionEntity> _deleteStrategy;
        private readonly IExistsStrategy<BroadcastedTransactionEntity> _existsStrategy;
        private readonly IGetAllStrategy<BroadcastedTransactionEntity> _getAllStrategy;

        public BroadcastedTransactionRepository(
            IAddStrategy<BroadcastedTransactionEntity> addStrategy,
            IDeleteStrategy<BroadcastedTransactionEntity> deleteStrategy,
            IExistsStrategy<BroadcastedTransactionEntity> existsStrategy,
            IGetAllStrategy<BroadcastedTransactionEntity> getAllStrategy)
        {
            _addStrategy    = addStrategy;
            _deleteStrategy = deleteStrategy;
            _existsStrategy = existsStrategy;
            _getAllStrategy = getAllStrategy;
        }


        public async Task AddAsync(BroadcastedTransactionDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.OperationId);
            entity.RowKey       = GetRowKey(dto.SignedTxData);

            await _addStrategy.ExecuteAsync(entity);
        }

        public async Task DeleteAsync(Guid operationId)
        {
            await _deleteStrategy.ExecuteAsync(GetPartitionKey(operationId));
        }

        public async Task<bool> ExistsAsync(Guid operation, string signedTxData)
        {
            return await _existsStrategy.ExecuteAsync
            (
                GetPartitionKey(operation),
                GetRowKey(signedTxData)
            );
        }

        public async Task<IEnumerable<BroadcastedTransactionDto>> GetAsync(Guid operationId)
        {
            return (await _getAllStrategy.ExecuteAsync(GetPartitionKey(operationId)))
                .Select(x => x.ToDto());
        }


        private static string GetPartitionKey(Guid operationId)
            => $"{operationId:N}";

        private static string GetRowKey(string signedTxData)
            => signedTxData.CalculateHexHash64();
    }
}
