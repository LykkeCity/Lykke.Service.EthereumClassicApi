using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Mappins
{
    internal static class ObservableBalanceLockMappings
    {
        public static ObservableBalanceLockDto ToDto(this ObservableBalanceLockEntity entity)
        {
            return new ObservableBalanceLockDto
            {
                Address = entity.Address
            };
        }

        public static ObservableBalanceLockEntity ToEntity(this ObservableBalanceLockDto dto)
        {
            return new ObservableBalanceLockEntity
            {
                Address = dto.Address
            };
        }
    }
}
