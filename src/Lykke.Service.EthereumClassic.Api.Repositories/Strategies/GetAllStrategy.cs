using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassic.Api.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Strategies
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
