using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;


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

        public async Task AddOrReplaceAsync(GasPriceDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey();
            entity.RowKey = GetRowKey();

            await _table.InsertOrReplaceAsync(entity);
        }

        public async Task<GasPriceDto> TryGetAsync()
        {
            return (await _table.GetDataAsync(GetPartitionKey(), GetRowKey()))?
                .ToDto();
        }
    }
}
