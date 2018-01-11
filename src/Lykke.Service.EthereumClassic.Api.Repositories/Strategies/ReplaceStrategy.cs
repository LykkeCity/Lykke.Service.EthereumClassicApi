using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassic.Api.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Strategies
{
    public class ReplaceStrategy<T> : IReplaceStrategy<T>
        where T : AzureTableEntity, new()
    {
        private readonly INoSQLTableStorage<T> _table;


        public ReplaceStrategy(
            INoSQLTableStorage<T> table)
        {
            _table = table;
        }


        public async Task ExecuteAsync(T entity)
        {
            await _table.ReplaceAsync(entity.PartitionKey, entity.RowKey, x => entity);
        }
    }
}
