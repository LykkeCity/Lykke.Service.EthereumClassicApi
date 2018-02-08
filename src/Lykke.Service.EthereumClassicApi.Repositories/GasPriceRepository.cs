using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class GasPriceRepository : IGasPriceRepository
    {
        private readonly INoSQLTableStorage<GasPriceEntity> _table;


        public GasPriceRepository(
            INoSQLTableStorage<GasPriceEntity> table)
        {
            _table = table;
        }


        private static string GetPartitionKey()
        {
            return "GasPrice";
        }

        private static string GetRowKey()
        {
            return "GasPrice";
        }

        public async Task AddOrReplaceAsync(GasPriceEntity entity)
        {
            entity.PartitionKey = GetPartitionKey();
            entity.RowKey = GetRowKey();

            await _table.InsertOrReplaceAsync(entity);
        }

        public async Task<GasPriceEntity> TryGetAsync()
        {
            return await _table.GetDataAsync(GetPartitionKey(), GetRowKey());
        }
    }
}
