using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Entities;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Mappins
{
    internal static class ObservableBalanceMappings
    {
        public static ObservableBalanceDto ToDto(this ObservableBalanceEntity entity)
        {
            return new ObservableBalanceDto
            {
                Address = entity.Address
            };
        }

        public static ObservableBalanceEntity ToEntity(this ObservableBalanceDto dto)
        {
            return new ObservableBalanceEntity
            {
                Address = dto.Address
            };
        }
    }
}
