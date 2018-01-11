using System.Numerics;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Entities;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Mappins
{
    internal static class OperationTransactionMappings
    {
        public static OperationTransactionDto ToDto(this OperationTransactionEntity entity)
        {
            return new OperationTransactionDto
            {
                Amount       = BigInteger.Parse(entity.Amount),
                CreatedOn    = entity.CreatedOn,
                Fee          = BigInteger.Parse(entity.Fee),
                FromAddress  = entity.FromAddress,
                OperationId  = entity.OperationId,
                SignedTxData = entity.SignedTxData,
                ToAddress    = entity.ToAddress,
                TxHash       = entity.TxHash
            };
        }

        public static OperationTransactionEntity ToEntity(this OperationTransactionDto dto)
        {
            return new OperationTransactionEntity
            {
                Amount       = dto.Amount.ToString(),
                CreatedOn    = dto.CreatedOn,
                Fee          = dto.Fee.ToString(),
                FromAddress  = dto.FromAddress,
                OperationId  = dto.OperationId,
                SignedTxData = dto.SignedTxData,
                ToAddress    = dto.ToAddress,
                TxHash       = dto.TxHash
            };
        }
    }
}
