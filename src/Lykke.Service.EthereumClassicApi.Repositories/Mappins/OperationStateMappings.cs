using System.Numerics;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Mappins
{
    internal static class OperationStateMappings
    {
        public static OperationStateDto ToDto(this OperationStateEntity entity)
        {
            return new OperationStateDto
            {
                Amount      = BigInteger.Parse(entity.Amount),
                Completed   = entity.Completed,
                Failed      = entity.Failed,
                FromAddress = entity.FromAddress,
                OperationId = entity.OperationId,
                Timestamp   = entity.Timestamp,
                ToAddress   = entity.ToAddress,
                TxHash      = entity.TxHash
            };
        }

        public static OperationStateEntity ToEntity(this OperationStateDto dto)
        {
            return new OperationStateEntity
            {
                Amount      = dto.Amount.ToString(),
                Completed   = dto.Completed,
                Failed      = dto.Failed,
                FromAddress = dto.FromAddress,
                OperationId = dto.OperationId,
                ToAddress   = dto.ToAddress,
                TxHash      = dto.TxHash
            };
        }
    }
}
