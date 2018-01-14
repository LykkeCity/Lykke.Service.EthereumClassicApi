using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies
{
    public class GetStrategy<T> : IGetStrategy<T>
        where T : AzureTableEntity, new()
    {
        private readonly INoSQLTableStorage<T> _table;


        public GetStrategy(
            INoSQLTableStorage<T> table)
        {
            _table = table;
        }


        public async Task<T> ExecuteAsync(string partitionKey, string rowKey)
        {
            return await _table.GetDataAsync(partitionKey, rowKey);
        }

        public async Task<IEnumerable<T>> ExecuteAsync(string partitionKey, int take, int skip)
        {
            return (await _table.GetTopRecordsAsync(partitionKey, skip + take))
                .Skip(skip)
                .Take(take);
        }
    }
}
