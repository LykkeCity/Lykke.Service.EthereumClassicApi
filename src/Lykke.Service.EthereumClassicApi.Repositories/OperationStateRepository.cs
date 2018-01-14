using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class OperationStateRepository : IOperationStateRepository
    {
        private readonly INoSQLTableStorage<OperationStateEntity> _table;

        public OperationStateRepository(
            INoSQLTableStorage<OperationStateEntity> table)
        {
            _table = table;
        }


        public async Task AddOrReplaceAsync(OperationStateDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.OperationId);
            entity.RowKey       = GetRowKey(entity.TxHash);

            await _table.InsertOrReplaceAsync(entity);
        }

        public async Task DeleteAsync(Guid operationId)
        {
            var operationTransactions = await _table.GetDataAsync(GetPartitionKey(operationId), x => true);

            await _table.DeleteAsync(operationTransactions);
        }

        public async Task DeleteInProgressAsync(Guid operationId)
        {
            var inProgressTransactions = await _table.GetDataAsync(GetPartitionKey(operationId), x => !x.Completed);

            await _table.DeleteAsync(inProgressTransactions);
        }

        public async Task<IEnumerable<OperationStateDto>> GetAsync(bool completed, bool failed, int take, string continuationToken)
        {
            return (await _table.GetDataAsync(x => x.Completed == completed && x.Failed == failed))
                .Select(x => x.ToDto());
        }


        private static string GetPartitionKey(Guid operationId)
            => $"{operationId:N}";

        private static string GetRowKey(string txHash)
            => txHash;
    }
}
