using System;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;


namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class BroadcastedTransactionStateRepository : IBroadcastedTransactionStateRepository
    {
        private readonly INoSQLTableStorage<BroadcastedTransactionStateEntity> _table;


        public BroadcastedTransactionStateRepository(
            INoSQLTableStorage<BroadcastedTransactionStateEntity> table)
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


        public async Task AddOrReplaceAsync(BroadcastedTransactionStateDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.OperationId);
            entity.RowKey = GetRowKey(dto.OperationId);

            await _table.InsertOrReplaceAsync(entity);
        }

        public async Task DeleteIfExistAsync(Guid operationId)
        {
            await _table.DeleteIfExistAsync
            (
                GetPartitionKey(operationId),
                GetRowKey(operationId)
            );
        }

        public async Task<bool> ExistsAsync(Guid operationId)
        {
            return await _table.GetDataAsync(GetPartitionKey(operationId), GetRowKey(operationId)) != null;
        }

        public async Task<BroadcastedTransactionStateDto> TryGetAsync(Guid operationId)
        {
            return (await _table.GetDataAsync(GetPartitionKey(operationId), GetRowKey(operationId)))?
                .ToDto();
        }
    }
}
