using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Services.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumClassicApi.Services.Tests.Extensions
{
    [TestClass]
    public class TransactionDtoExtensionsTests
    {
        [DataTestMethod]
        [DataRow("10000000000000000", "50000000000", "20000000000", false, "462000000000000", "10000000000000000")]
        [DataRow("10000000000000000", "50000000000", "20000000000", true, "462000000000000",   "9538050000000000")]
        public void CalculateTransactionParams__ExpectedResultReturned(
            string amount, string fee, string gasPrice, bool includeFee, string expectedFee, string expectedAmount)
        {
            var feeFactor = 1.1m;
            TransactionDto dto = new TransactionDto()
            {
                Amount = BigInteger.Parse(amount),
                Fee = BigInteger.Parse(fee),
                GasPrice = BigInteger.Parse(gasPrice),
                IncludeFee = includeFee
            };

            var trParams = dto.CalculateTransactionParams(feeFactor);

            var expectedFeeResult = BigInteger.Parse(expectedFee);
            var actualFeeResult = trParams.Fee;

            var expectedAmountResult = BigInteger.Parse(expectedAmount);
            var actualAmountResult = trParams.Amount;

            Assert.AreEqual(expectedFeeResult, actualFeeResult);
            Assert.AreEqual(expectedAmountResult, actualAmountResult);
        }
    }
}
