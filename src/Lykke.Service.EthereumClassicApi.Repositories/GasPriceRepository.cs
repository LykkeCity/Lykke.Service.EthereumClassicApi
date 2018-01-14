using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class GasPriceRepository : IGasPriceRepository
    {
        private readonly IAddOrReplaceStrategy<GasPriceEntity> _addOrReplaceStrategy;
        private readonly IGetStrategy<GasPriceEntity>          _getStrategy;

        public GasPriceRepository(
            IAddOrReplaceStrategy<GasPriceEntity> addOrReplaceStrategy,
            IGetStrategy<GasPriceEntity> getStrategy)
        {
            _addOrReplaceStrategy = addOrReplaceStrategy;
            _getStrategy          = getStrategy;
        }

        public async Task AddOrReplaceAsync(GasPriceDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey();
            entity.RowKey       = GetRowKey();

            await _addOrReplaceStrategy.ExecuteAsync(entity);
        }

        public async Task<GasPriceDto> GetAsync()
        {
            var entity = await _getStrategy.ExecuteAsync(GetPartitionKey(), GetRowKey());

            return entity?.ToDto();
        }
        

        private static string GetPartitionKey()
            => "GasPrice";

        private static string GetRowKey()
            => "GasPrice";
    }
}
