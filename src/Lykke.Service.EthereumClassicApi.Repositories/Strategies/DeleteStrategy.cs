using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies
{
    public class DeleteStrategy<T> : IDeleteStrategy<T>
        where T : AzureTableEntity, new()
    {
        private readonly INoSQLTableStorage<T> _table;

        public DeleteStrategy(
            INoSQLTableStorage<T> table)
        {
            _table = table;
        }


        public async Task ExecuteAsync(string partitionKey)
        {
            var entities = await _table.GetDataAsync(partitionKey, x => true);

            await Task.WhenAll
            (
                entities.Select(x => _table.DeleteIfExistAsync(x.PartitionKey, x.RowKey))
            );
        }

        public async Task ExecuteAsync(string partitionKey, string rowKey)
        {
            await _table.DeleteIfExistAsync(partitionKey, rowKey);
        }
    }
}
