using System.Numerics;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Mappins
{
    internal static class GasPriceMappings
    {
        public static GasPriceDto ToDto(this GasPriceEntity entity)
        {
            return new GasPriceDto
            {
                Max = BigInteger.Parse(entity.Max),
                Min = BigInteger.Parse(entity.Min)
            };
        }

        public static GasPriceEntity ToEntity(this GasPriceDto dto)
        {
            return new GasPriceEntity
            {
                Max = dto.Max.ToString(),
                Min = dto.Min.ToString()
            };
        }
    }
}
