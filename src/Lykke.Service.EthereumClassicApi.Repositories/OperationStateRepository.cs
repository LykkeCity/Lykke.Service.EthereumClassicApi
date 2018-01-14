using System;
using System.Threading.Tasks;
using Common;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class OperationStateRepository : IOperationStateRepository
    {
        private readonly IAddOrReplaceStrategy<OperationStateEntity> _addOrReplaceStrategy;
        private readonly IDeleteStrategy<OperationStateEntity>       _deleteStrategy;
        private readonly IGetStrategy<OperationStateEntity>          _getStrategy;


        public OperationStateRepository(
            IAddOrReplaceStrategy<OperationStateEntity> addOrReplaceStrategy,
            IDeleteStrategy<OperationStateEntity> deleteStrategy,
            IGetStrategy<OperationStateEntity> getStrategy)
        {
            _addOrReplaceStrategy = addOrReplaceStrategy;
            _deleteStrategy       = deleteStrategy;
            _getStrategy          = getStrategy;
        }


        public async Task AddOrReplaceAsync(OperationStateDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.OperationId);
            entity.RowKey       = GetRowKey(dto.OperationId);
            
            await _addOrReplaceStrategy.ExecuteAsync(entity);
        }

        public async Task DeleteAsync(Guid operationId)
        {
            await _deleteStrategy.ExecuteAsync(GetPartitionKey(operationId));
        }

        public async Task<OperationStateDto> GetAsync(Guid operationId)
        {
            return (await _getStrategy.ExecuteAsync(GetPartitionKey(operationId), GetRowKey(operationId)))?
                .ToDto();
        }
        

        private static string GetPartitionKey(Guid operationId)
            => operationId.ToString().CalculateHexHash32(3);

        private static string GetRowKey(Guid operationId)
            => $"{operationId:N}";
    }
}
