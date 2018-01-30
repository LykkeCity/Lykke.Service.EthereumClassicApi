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
                Amount = BigInteger.Parse(entity.Amount),
                Locked = entity.Locked
            };
        }
    }
}
