using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies
{
    public class GetAllStrategy<T> : IGetAllStrategy<T>
        where T : AzureTableEntity, new()
    {
        private readonly INoSQLTableStorage<T> _table;


        public GetAllStrategy(
            INoSQLTableStorage<T> table)
        {
            _table = table;
        }


        public async Task<IEnumerable<T>> ExecuteAsync(string partitionKey)
        {
            return await _table.GetDataAsync(partitionKey);
        }
    }
}
