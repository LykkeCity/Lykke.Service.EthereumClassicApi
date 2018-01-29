using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies
{
    public class ExistsStrategy<T> : IExistsStrategy<T>
        where T : AzureTableEntity, new()
    {
        private readonly INoSQLTableStorage<T> _table;


        public ExistsStrategy(
            INoSQLTableStorage<T> table)
        {
            _table = table;
        }

        public async Task<bool> ExecuteAsync(string partitionKey, string rowKey)
        {
            return await _table.GetDataAsync(partitionKey, rowKey) != null;
        }
    }
}
