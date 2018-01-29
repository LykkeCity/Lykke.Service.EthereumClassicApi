using System.Numerics;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Mappins
{
    internal static class BuiltTransactionMappings
    {
        public static BuiltTransactionDto ToDto(this BuiltTransactionEntity entity)
        {
            return new BuiltTransactionDto
            {
                Amount = BigInteger.Parse(entity.Amount),
                FromAddress = entity.FromAddress,
                GasPrice = BigInteger.Parse(entity.GasPrice),
                IncludeFee = entity.IncludeFee,
                Nonce = BigInteger.Parse(entity.Nonce),
                OperationId = entity.OperationId,
                ToAddress = entity.ToAddress
            };
        }

        public static BuiltTransactionEntity ToEntity(this BuiltTransactionDto dto)
        {
            return new BuiltTransactionEntity
            {
                Amount = dto.Amount.ToString(),
                FromAddress = dto.FromAddress,
                GasPrice = dto.GasPrice.ToString(),
                IncludeFee = dto.IncludeFee,
                Nonce = dto.Nonce.ToString(),
                OperationId = dto.OperationId,
                ToAddress = dto.ToAddress
            };
        }
    }
}
