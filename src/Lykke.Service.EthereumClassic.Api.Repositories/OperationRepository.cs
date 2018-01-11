using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Entities;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Repositories.Mappins;
using Lykke.Service.EthereumClassic.Api.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Repositories
{
    public class OperationRepository : IOperationRepository
    {
        private readonly IAddStrategy<OperationEntity>    _addStrategy;
        private readonly IDeleteStrategy<OperationEntity> _deleteStrategy;
        private readonly IGetAllStrategy<OperationEntity> _getAllStrategy;
        private readonly IGetStrategy<OperationEntity>    _getStrategy;


        public OperationRepository(
            IAddStrategy<OperationEntity> addStrategy,
            IDeleteStrategy<OperationEntity> deleteStrategy,
            IGetAllStrategy<OperationEntity> getAllStrategy,
            IGetStrategy<OperationEntity> getStrategy)
        {
            _addStrategy    = addStrategy;
            _deleteStrategy = deleteStrategy;
            _getAllStrategy = getAllStrategy;
            _getStrategy    = getStrategy;
        }


        public async Task AddAsync(OperationDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey();
            entity.RowKey       = GetRowKey(dto.OperationId);

            await _addStrategy.ExecuteAsync(entity);
        }

        public async Task DeleteAsync(Guid operationId)
        {
            await _deleteStrategy.ExecuteAsync(GetPartitionKey(), GetRowKey(operationId));
        }

        public async Task<OperationDto> GetAsync(Guid operationId)
        {
            return (await _getStrategy.ExecuteAsync(GetPartitionKey(), GetRowKey(operationId)))
                .ToDto();
        }

        public async Task<IEnumerable<Guid>> GetAllOperationIdsAsync()
        {
            return (await _getAllStrategy.ExecuteAsync(GetPartitionKey()))
                .Select(x => x.OperationId);
        }

        private static string GetPartitionKey()
            => "Operation";

        private static string GetRowKey(Guid operationId)
            => $"{operationId:N}";
    }
}
