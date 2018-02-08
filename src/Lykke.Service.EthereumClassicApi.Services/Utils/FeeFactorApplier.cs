using System;
using System.Numerics;
using JetBrains.Annotations;

namespace Lykke.Service.EthereumClassicApi.Services.Utils
{
    internal static class FeeFactorApplier
    {
        [Pure]
        public static BigInteger Apply(BigInteger gasPrice, decimal feeFactor)
        {
            if (gasPrice <= 0)
            {
                throw new ArgumentException("Gas price should be grater then zero.", nameof(gasPrice));
            }

            if (feeFactor <= 1)
            {
                throw new ArgumentException("Fee factor should be grater then one.", nameof(feeFactor));
            }
            
            var feeFactorBits = decimal.GetBits(feeFactor);
            var feeFactorMultiplier = new BigInteger(new decimal(feeFactorBits[0], feeFactorBits[1], feeFactorBits[2], false, 0));
            var decimalPlacesNumber = (int)BitConverter.GetBytes(feeFactorBits[3])[2];
            var feeFactorDivider = new BigInteger(Math.Pow(10, decimalPlacesNumber));
            var newGasPrice = gasPrice * feeFactorMultiplier / feeFactorDivider;

            if (newGasPrice > gasPrice)
            {
                return newGasPrice;
            }

            return gasPrice + 1;
        }
    }
}
