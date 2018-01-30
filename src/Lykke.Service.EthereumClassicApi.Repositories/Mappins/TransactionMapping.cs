using System.Numerics;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Mappins
{
    internal static class TransactionMapping
    {
        public static TransactionDto ToDto(this TransactionEntity entity)
        {
            return new TransactionDto
            {
                Amount = BigInteger.Parse(entity.Amount),
                BroadcastedOn = entity.BroadcastedOn,
                BuiltOn = entity.BuiltOn,
                CompletedOn = entity.CompletedOn,
                Error = entity.Error,
                Fee = BigInteger.Parse(entity.Fee),
                FromAddress = entity.FromAddress,
                GasPrice = BigInteger.Parse(entity.GasPrice),
                IncludeFee = entity.IncludeFee,
                Nonce = BigInteger.Parse(entity.Nonce),
                OperationId = entity.OperationId,
                SignedTxData = entity.SignedTxData,
                SignedTxHash = entity.SignedTxHash,
                State = EnumUtil.Parse<TransactionState>(entity.State),
                ToAddress = entity.ToAddress,
                TxData = entity.TxData
            };
        }
    }
}
