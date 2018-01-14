using System.Numerics;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Mappins
{
    internal static class OperationMappings
    {
        public static OperationDto ToDto(this OperationEntity entity)
        {
            return new OperationDto
            {
                Amount      = BigInteger.Parse(entity.Amount),
                FromAddress = entity.FromAddress,
                GasPrice    = BigInteger.Parse(entity.GasPrice),
                IncludeFee  = entity.IncludeFee,
                Nonce       = BigInteger.Parse(entity.Nonce),
                OperationId = entity.OperationId,
                ToAddress   = entity.ToAddress
            };
        }

        public static OperationEntity ToEntity(this OperationDto dto)
        {
            return new OperationEntity
            {
                Amount      = dto.Amount.ToString(),
                FromAddress = dto.FromAddress,
                GasPrice    = dto.GasPrice.ToString(),
                IncludeFee  = dto.IncludeFee,
                Nonce       = dto.Nonce.ToString(),
                OperationId = dto.OperationId,
                ToAddress   = dto.ToAddress
            };
        }
    }
}
