using Lykke.Common.Chaos;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Extensions;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Services.Tests.Extensions
{
    [TestClass]
    public class TransactionServiceTest
    {
        public static BigInteger gasPrice = BigInteger.Parse("20000000000");

        [DataTestMethod]
        [DataRow("1000000000000000", "1000000000000000", "420000000000000", false)]
        [DataRow("1000000000000000", "580000000000000", "420000000000000", true)]
        public void CalculateTransactionParamsAsync__ExpectedResultReturned(
            string amountStr,
            string expectedAmountStr,
            string expectedFeeStr,
            bool includeFee)
        {
            string to = "0x83F0726180Cf3964b69f62AC063C5Cb9A66B3bE5";
            BigInteger amount = BigInteger.Parse(amountStr);
            BigInteger expectedAmount = BigInteger.Parse(expectedAmountStr);
            BigInteger expectedFee = BigInteger.Parse(expectedFeeStr);
            var service = InitTransactionService(to, amount);

            //ACT
            var actualCalculated = service.CalculateTransactionParamsAsync(amount, includeFee, to).Result;

            //ASSERT
            Assert.AreEqual(expectedAmount, actualCalculated.Amount);
            Assert.AreEqual(expectedFee, actualCalculated.Fee);
            Assert.AreEqual(gasPrice, actualCalculated.GasPrice);
        }

        [TestMethod]
        public void CalculateTransactionParamsAsync__On_Zero_Amount__Throws_Exception()
        {
            string to = "0x83F0726180Cf3964b69f62AC063C5Cb9A66B3bE5";
            BigInteger amount = 0;

            TransactionService service = InitTransactionService(to, amount);

            Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                var actualCalculated = await service.CalculateTransactionParamsAsync(amount, true, to);
            }).Wait();
        }

        [TestMethod]
        public void CalculateTransactionParamsAsync__On_Wrong_ToAddress__Throws_Exception()
        {
            string to = "///";
            BigInteger amount = 10;

            TransactionService service = InitTransactionService(to, amount);

            Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                var actualCalculated = await service.CalculateTransactionParamsAsync(amount, true, to);
            }).Wait();
        }

        private static TransactionService InitTransactionService(string to, BigInteger amount)
        {
            #region Mock

            Mock<IEthereum> ethereum = new Mock<IEthereum>();
            Mock<IGasPriceOracleService> gasPriceOracleService = new Mock<IGasPriceOracleService>();
            Mock<IObservableBalanceRepository> observableBalanceRepository = new Mock<IObservableBalanceRepository>();
            Mock<ITransactionRepository> transactionRepository = new Mock<ITransactionRepository>();
            Mock<IChaosKitty> chaosKitty = new Mock<IChaosKitty>();

            gasPriceOracleService.Setup(x => x.CalculateGasPriceAsync(to, amount)).Returns(Task.FromResult(gasPrice));

            #endregion

            TransactionService service = new TransactionService(
                ethereum.Object,
                gasPriceOracleService.Object,
                observableBalanceRepository.Object,
                transactionRepository.Object,
                chaosKitty.Object);

            return service;
        }
    }
}
