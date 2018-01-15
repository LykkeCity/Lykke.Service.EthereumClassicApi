using System.Numerics;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Mappins
{
    internal static class BroadcastedTransactionStateMappings
    {
        public static BroadcastedTransactionStateDto ToDto(this BroadcastedTransactionStateEntity entity)
        {
            return new BroadcastedTransactionStateDto
            {
                Amount      = BigInteger.Parse(entity.Amount),
                FromAddress = entity.FromAddress,
                OperationId = entity.OperationId,
                State       = EnumUtil.Parse<TransactionState>(entity.State), 
                Timestamp   = entity.Timestamp,
                ToAddress   = entity.ToAddress,
                TxHash      = entity.TxHash
            };
        }

        public static BroadcastedTransactionStateEntity ToEntity(this BroadcastedTransactionStateDto dto)
        {
            return new BroadcastedTransactionStateEntity
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
