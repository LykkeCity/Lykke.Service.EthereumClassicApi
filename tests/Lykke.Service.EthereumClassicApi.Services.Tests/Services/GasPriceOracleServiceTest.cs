using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Extensions;
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
    public class GasPriceOracleServiceTest
    {
        public const string DefaultMinGasPrice = "20000000000";
        public const string DefaultMaxGasPrice = "80000000000";

        [DataTestMethod]
        [DataRow(DefaultMinGasPrice, DefaultMaxGasPrice, "90000000000", DefaultMaxGasPrice, true)]
        [DataRow(DefaultMinGasPrice, DefaultMaxGasPrice, "90000000000", DefaultMaxGasPrice, false)]
        [DataRow(DefaultMinGasPrice, DefaultMaxGasPrice, "10000000000", DefaultMinGasPrice, true)]
        [DataRow(DefaultMinGasPrice, DefaultMaxGasPrice, "10000000000", DefaultMinGasPrice, false)]
        [DataRow(DefaultMinGasPrice, DefaultMaxGasPrice, "30000000000", "30000000000", true)]
        [DataRow(DefaultMinGasPrice, DefaultMaxGasPrice, "30000000000", "30000000000", false)]
        public void CalculateGasPriceAsync__ExpectedResultReturned(
            string defaultMinGasPrice,
            string defaultMaxGasPrice,
            string estimatedResultStr,
            string expectedResultStr,
            bool isSettingsStored)
        {
            string to = "0x83F0726180Cf3964b69f62AC063C5Cb9A66B3bE5";
            BigInteger amount = 1000000000;
            BigInteger estimatedResult = BigInteger.Parse(estimatedResultStr);
            BigInteger expectedResult = BigInteger.Parse(expectedResultStr);
            GasPriceEntity newGasPriceDto = new GasPriceEntity()
            {
                Max = BigInteger.Parse(defaultMaxGasPrice),
                Min = BigInteger.Parse(defaultMinGasPrice),
            };
            GasPriceEntity gasPriceDto = isSettingsStored ? newGasPriceDto : null;

            #region Mock

            EthereumClassicApiSettings settings = new EthereumClassicApiSettings();
            Mock<IEthereum> ethereum = new Mock<IEthereum>();
            Mock<IGasPriceRepository> gasPriceRepository = new Mock<IGasPriceRepository>();

            settings.DefaultMinGasPrice = defaultMinGasPrice;
            settings.DefaultMaxGasPrice = defaultMaxGasPrice;

            ethereum.Setup(x => x.EstimateGasPriceAsync(to, amount)).Returns( Task.FromResult(estimatedResult));

            gasPriceRepository.Setup(x => x.TryGetAsync()).Returns(Task.FromResult(gasPriceDto));
            gasPriceRepository.Setup(x => x.AddOrReplaceAsync(It.IsAny<GasPriceEntity>()))
                .Returns(Task.FromResult(0)).Verifiable();

            #endregion

            GasPriceOracleService service = new GasPriceOracleService(settings, ethereum.Object, gasPriceRepository.Object);

            //ACT
            var actualCalculated = service.CalculateGasPriceAsync(to, amount).Result;

            //ASSERT
            Assert.AreEqual(expectedResult, actualCalculated);

            if (gasPriceDto == null)
            {
                gasPriceRepository.Verify(x => x.AddOrReplaceAsync(It.IsAny<GasPriceEntity>()), Times.Once());
            }
        }
    }
}
