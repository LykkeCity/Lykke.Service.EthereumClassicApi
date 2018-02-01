//using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
//using Lykke.Service.EthereumClassicApi.Common.Settings;
//using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
//using Lykke.Service.EthereumClassicApi.Repositories.Entities;
//using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
//using Lykke.Service.EthereumClassicApi.Services.Extensions;
//using Lykke.Service.EthereumClassicApi.Services.Interfaces;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Numerics;
//using System.Text;
//using System.Threading.Tasks;

//namespace Lykke.Service.EthereumClassicApi.Services.Tests.Extensions
//{
//    [TestClass]
//    public class TransactionServiceTest
//    {
//        public static BigInteger gasPrice = BigInteger.Parse("20000000000");

//        [DataTestMethod]
//        //[DataRow()]
//        public void CalculateGasPriceAsync__ExpectedResultReturned(
//            string defaultMinGasPrice,
//            string defaultMaxGasPrice,
//            string estimatedResultStr,
//            string expectedAmountStr,
//            bool includeFee)
//        {
//            string to = "0x83F0726180Cf3964b69f62AC063C5Cb9A66B3bE5";
//            BigInteger amount = 1000000000;
//            BigInteger estimatedResult = BigInteger.Parse(estimatedResultStr);
//            BigInteger expectedAmount = BigInteger.Parse(expectedAmountStr);
//            BigInteger expectedFee = BigInteger.Parse(expectedAmountStr);

//            #region Mock

//            Mock<IEthereum> ethereum = new Mock<IEthereum>();
//            Mock<IGasPriceOracleService> gasPriceOracleService = new Mock<IGasPriceOracleService>();
//            Mock<IObservableBalanceRepository> observableBalanceRepository = new Mock<IObservableBalanceRepository>();
//            Mock<ITransactionRepository> transactionRepository = new Mock<ITransactionRepository>();

//            gasPriceOracleService.Setup(x => x.CalculateGasPriceAsync(to, amount)).Returns(Task.FromResult(gasPrice));

//            //gasPriceRepository.Setup(x => x.TryGetAsync()).Returns(Task.FromResult(gasPriceDto));
//            //gasPriceRepository.Setup(x => x.AddOrReplaceAsync(It.IsAny<GasPriceEntity>()))
//            //    .Returns(Task.FromResult(0)).Verifiable();

//            #endregion

//            TransactionService service = new TransactionService(
//                ethereum.Object,
//                gasPriceOracleService.Object,
//                observableBalanceRepository.Object,
//                transactionRepository.Object);

//            //ACT
//            var actualCalculated = service.CalculateTransactionParamsAsync(amount, includeFee, to).Result;

//            //ASSERT
//            Assert.AreEqual(expectedAmount, actualCalculated.Amount);
//            Assert.AreEqual(expectedAmount, actualCalculated.Fee);
//            Assert.AreEqual(expectedAmount, actualCalculated.GasPrice);

//            //if (gasPriceDto == null)
//            //{
//            //    gasPriceRepository.Verify(x => x.AddOrReplaceAsync(It.IsAny<GasPriceEntity>()), Times.Once());
//            //}
//        }
//    }
//}
