using System;
using System.Globalization;
using System.Numerics;
using Lykke.Service.EthereumClassicApi.Services.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.EthereumClassicApi.Services.Tests.Utils
{
    [TestClass]
    public class FeeFactorApplierTests
    {
        [DataTestMethod]
        [DataRow("1", "1.1", "2")]
        [DataRow("10", "1.1", "11")]
        public void Apply__ExpectedResultReturned(string gasPriceString, string feeFactorString, string expectedResultString)
        {
            var gasPrice = BigInteger.Parse(gasPriceString);
            var feeFactor = decimal.Parse(feeFactorString, CultureInfo.InvariantCulture);
            var expectedResult = BigInteger.Parse(expectedResultString);

            var actualResult = FeeFactorApplier.Apply(gasPrice, feeFactor);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Apply__GasPrice_Lower_Then_One_Passed__ArgumentAxceptionThrown()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var gasPrice = new BigInteger(0);
                var feeFactor = 1.1m;

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                FeeFactorApplier.Apply(gasPrice, feeFactor);
            });
        }

        [TestMethod]
        public void Apply__FeeFactor_Lower_Then_One_Passed__ArgumentAxceptionThrown()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var gasPrice = new BigInteger(1);
                var feeFactor = 0.9m;

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                FeeFactorApplier.Apply(gasPrice, feeFactor);
            });
        }
    }
}
