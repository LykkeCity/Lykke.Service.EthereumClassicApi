using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassic.Api.Repositories.Strategies.Interfaces;


namespace Lykke.Service.EthereumClassic.Api.Repositories.Strategies
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


        public async Task ExecuteAsync(string partitionKey, string rowKey)
        {
            await _table.DeleteIfExistAsync(partitionKey, rowKey);
        }

        public async Task ExecuteAsync(string partitionKey, IEnumerable<string> rowKeys)
        {
            var tasks = rowKeys.Select(x => _table.DeleteIfExistAsync(partitionKey, x));

            await Task.WhenAll(tasks);
        }
    }
}
