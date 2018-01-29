using System;
using System.Threading.Tasks;
using Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class BroadcastedTransactionStateRepository : IBroadcastedTransactionStateRepository
    {
        private readonly IAddOrReplaceStrategy<BroadcastedTransactionStateEntity> _addOrReplaceStrategy;
        private readonly IDeleteStrategy<BroadcastedTransactionStateEntity> _deleteStrategy;
        private readonly IGetStrategy<BroadcastedTransactionStateEntity> _getStrategy;


        public BroadcastedTransactionStateRepository(
            IAddOrReplaceStrategy<BroadcastedTransactionStateEntity> addOrReplaceStrategy,
            IDeleteStrategy<BroadcastedTransactionStateEntity> deleteStrategy,
            IGetStrategy<BroadcastedTransactionStateEntity> getStrategy)
        {
            _addOrReplaceStrategy = addOrReplaceStrategy;
            _deleteStrategy = deleteStrategy;
            _getStrategy = getStrategy;
        }


        private static string GetPartitionKey(Guid operationId)
        {
            return operationId.ToString().CalculateHexHash32(3);
        }

        private static string GetRowKey(Guid operationId)
        {
            return $"{operationId:N}";
        }


        public async Task AddOrReplaceAsync(BroadcastedTransactionStateDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.OperationId);
            entity.RowKey = GetRowKey(dto.OperationId);

            await _addOrReplaceStrategy.ExecuteAsync(entity);
        }

        public async Task DeleteAsync(Guid operationId)
        {
            await _deleteStrategy.ExecuteAsync(GetPartitionKey(operationId));
        }

        public async Task<BroadcastedTransactionStateDto> GetAsync(Guid operationId)
        {
            return (await _getStrategy.ExecuteAsync(GetPartitionKey(operationId), GetRowKey(operationId)))?
                .ToDto();
        }
    }
}
