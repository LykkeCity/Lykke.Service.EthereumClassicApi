using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies
{
    public class AddOrReplaceStrategy<T> : IAddOrReplaceStrategy<T>
        where T : AzureTableEntity, new()
    {
        private readonly INoSQLTableStorage<T> _table;

        public AddOrReplaceStrategy(
            INoSQLTableStorage<T> table)
        {
            _table = table;
        }

        public async Task ExecuteAsync(T entity)
        {
            await _table.InsertOrReplaceAsync(entity);
        }
    }
}
