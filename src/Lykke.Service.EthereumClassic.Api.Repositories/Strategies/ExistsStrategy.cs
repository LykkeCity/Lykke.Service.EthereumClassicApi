using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassic.Api.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Strategies
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
            return (await _table.GetDataAsync(partitionKey, rowKey)) != null;
        }
    }
}
