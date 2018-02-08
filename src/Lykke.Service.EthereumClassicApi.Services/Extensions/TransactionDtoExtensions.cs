using JetBrains.Annotations;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Services.DTOs;
using Lykke.Service.EthereumClassicApi.Services.Utils;

namespace Lykke.Service.EthereumClassicApi.Services.Extensions
{
    internal static class TransactionDtoExtensions
    {
        [Pure]
        public static TransactionParamsDto CalculateTransactionParams(this TransactionEntity initialTransaction, decimal feeFactor)
        {
            var amount = initialTransaction.Amount;
            var fee = initialTransaction.Fee;
            var gasPrice = initialTransaction.GasPrice;
            var includeFee = initialTransaction.IncludeFee;


            if (includeFee)
            {
                amount += fee;
            }
            
            gasPrice = FeeFactorApplier.Apply(gasPrice, feeFactor);
            fee = gasPrice * Constants.EtcTransferGasAmount;

            if (includeFee)
            {
                amount -= fee;
            }

            return new TransactionParamsDto
            {
                Amount = amount,
                Fee = fee,
                GasPrice = gasPrice
            };
        }
    }
}
