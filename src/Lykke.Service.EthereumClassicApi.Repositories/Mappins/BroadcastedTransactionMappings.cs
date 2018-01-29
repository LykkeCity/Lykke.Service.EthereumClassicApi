using System.Numerics;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Mappins
{
    internal static class BroadcastedTransactionMappings
    {
        public static BroadcastedTransactionDto ToDto(this BroadcastedTransactionEntity entity)
        {
            return new BroadcastedTransactionDto
            {
                Amount = BigInteger.Parse(entity.Amount),
                Timestamp = entity.CreatedOn,
                FromAddress = entity.FromAddress,
                OperationId = entity.OperationId,
                SignedTxData = entity.SignedTxData,
                ToAddress = entity.ToAddress,
                TxHash = entity.TxHash
            };
        }

        public static BroadcastedTransactionEntity ToEntity(this BroadcastedTransactionDto dto)
        {
            return new BroadcastedTransactionEntity
            {
                Amount = dto.Amount.ToString(),
                CreatedOn = dto.Timestamp,
                FromAddress = dto.FromAddress,
                OperationId = dto.OperationId,
                SignedTxData = dto.SignedTxData,
                ToAddress = dto.ToAddress,
                TxHash = dto.TxHash
            };
        }
    }
}
