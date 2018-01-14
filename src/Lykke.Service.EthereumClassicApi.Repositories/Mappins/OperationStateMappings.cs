using System.Numerics;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Utils;
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
                FromAddress = entity.FromAddress,
                OperationId = entity.OperationId,
                State       = EnumUtil.Parse<OperationState>(entity.State), 
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
                FromAddress = dto.FromAddress,
                OperationId = dto.OperationId,
                State       = dto.State.ToString(),
                ToAddress   = dto.ToAddress,
                TxHash      = dto.TxHash
            };
        }
    }
}
