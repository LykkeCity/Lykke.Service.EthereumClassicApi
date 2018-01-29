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
    public class BuiltTransactionRepository : IBuiltTransactionRepository
    {
        private readonly INoSQLTableStorage<BuiltTransactionEntity> _table;


        public BuiltTransactionRepository(
            INoSQLTableStorage<BuiltTransactionEntity> table)
        {
            _table = table;
        }

        private static string GetPartitionKey(Guid operationId)
        {
            return operationId.ToString().CalculateHexHash32(3);
        }

        private static string GetRowKey(Guid operationId)
        {
            return operationId.ToString();
        }


        public async Task AddAsync(BuiltTransactionDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.OperationId);
            entity.RowKey = GetRowKey(dto.OperationId);

            await _table.InsertAsync(entity);
        }

        public async Task DeleteIfExistsAsync(Guid operationId)
        {
            await _table.DeleteIfExistAsync
            (
                GetPartitionKey(operationId),
                GetRowKey(operationId)
            );
        }

        public async Task<BuiltTransactionDto> TryGetAsync(Guid operationId)
        {
            return (await _table.GetDataAsync(GetPartitionKey(operationId), GetRowKey(operationId)))?
                .ToDto();
        }
    }
}
