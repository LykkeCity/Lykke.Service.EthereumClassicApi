﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class BuiltTransactionRepository : IBuiltTransactionRepository
    {
        private readonly IAddStrategy<BuiltTransactionEntity>    _addStrategy;
        private readonly IDeleteStrategy<BuiltTransactionEntity> _deleteStrategy;
        private readonly IGetAllStrategy<BuiltTransactionEntity> _getAllStrategy;
        private readonly IGetStrategy<BuiltTransactionEntity>    _getStrategy;


        public BuiltTransactionRepository(
            IAddStrategy<BuiltTransactionEntity> addStrategy,
            IDeleteStrategy<BuiltTransactionEntity> deleteStrategy,
            IGetAllStrategy<BuiltTransactionEntity> getAllStrategy,
            IGetStrategy<BuiltTransactionEntity> getStrategy)
        {
            _addStrategy    = addStrategy;
            _deleteStrategy = deleteStrategy;
            _getAllStrategy = getAllStrategy;
            _getStrategy    = getStrategy;
        }


        public async Task AddAsync(BuiltTransactionDto dto)
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

        public async Task<BuiltTransactionDto> GetAsync(Guid operationId)
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