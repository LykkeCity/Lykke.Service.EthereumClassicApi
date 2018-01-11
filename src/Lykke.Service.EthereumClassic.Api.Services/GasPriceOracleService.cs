using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Blockchain.Interfaces;
using Lykke.Service.EthereumClassic.Api.Common.Settings;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Services.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Services
{
    public class GasPriceOracleService : IGasPriceOracleService
    {
        private readonly BigInteger          _defaultMaxGasPrice;
        private readonly BigInteger          _defaultMinGasPrice;
        private readonly IEthereum           _ethereum;
        private readonly IGasPriceRepository _gasPriceRepository;



        public GasPriceOracleService(
            EthereumClassicApiSettings serviceSettings,
            IEthereum ethereum,
            IGasPriceRepository gasPriceRepository)
        {
            _defaultMaxGasPrice = BigInteger.Parse(serviceSettings.DefaultMaxGasPrice);
            _defaultMinGasPrice = BigInteger.Parse(serviceSettings.DefaultMinGasPrice);
            _ethereum           = ethereum;
            _gasPriceRepository = gasPriceRepository;
        }


        public async Task<BigInteger> CalculateGasPriceAsync(string to, BigInteger amount)
        {
            var estimatedGasPrice = await _ethereum.EstimateGasPriceAsync(to, amount);
            var minMaxGasPrice    = await _gasPriceRepository.GetAsync();

            if (minMaxGasPrice == null)
            {
                minMaxGasPrice = new GasPriceDto
                {
                    Max = _defaultMaxGasPrice,
                    Min = _defaultMinGasPrice
                };

                await _gasPriceRepository.AddOrReplaceAsync(minMaxGasPrice);
            }

            if (estimatedGasPrice <= minMaxGasPrice.Min)
            {
                return minMaxGasPrice.Min;
            }

            if (estimatedGasPrice >= minMaxGasPrice.Max)
            {
                return minMaxGasPrice.Max;
            }

            return estimatedGasPrice;
        }
    }
}
