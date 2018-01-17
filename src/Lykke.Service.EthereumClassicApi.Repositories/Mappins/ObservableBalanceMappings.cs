using System.Numerics;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Mappins
{
    internal static class ObservableBalanceMappings
    {
        public static ObservableBalanceDto ToDto(this ObservableBalanceEntity entity)
        {
            return new ObservableBalanceDto
            {
                Address = entity.Address,
                Amount  = BigInteger.Parse(entity.Amount)
            };
        }

        public static ObservableBalanceEntity ToEntity(this ObservableBalanceDto dto)
        {
            return new ObservableBalanceEntity
            {
                Address = dto.Address,
                Amount  = dto.Amount.ToString()
            };
        }
    }
}
