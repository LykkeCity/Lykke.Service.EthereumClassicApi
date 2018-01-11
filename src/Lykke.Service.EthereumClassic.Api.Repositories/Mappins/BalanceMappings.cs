using System.Numerics;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Entities;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Mappins
{
    internal static class BalanceMappings
    {
        public static BalanceDto ToDto(this BalanceEntity entity)
        {
            return new BalanceDto
            {
                Address = entity.Address,
                Balance = BigInteger.Parse(entity.Balance)
            };
        }

        public static BalanceEntity ToEntity(this BalanceDto dto)
        {
            return new BalanceEntity
            {
                Address = dto.Address,
                Balance = dto.Balance.ToString()
            };
        }
    }
}
