using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Services
{
    public class GasPriceOracleService : IGasPriceOracleService
    {
        private readonly BigInteger _defaultMaxGasPrice;
        private readonly BigInteger _defaultMinGasPrice;
        private readonly IEthereum _ethereum;
        private readonly IGasPriceRepository _gasPriceRepository;


        public GasPriceOracleService(
            EthereumClassicApiSettings serviceSettings,
            IEthereum ethereum,
            IGasPriceRepository gasPriceRepository)
        {
            _defaultMaxGasPrice = BigInteger.Parse(serviceSettings.DefaultMaxGasPrice);
            _defaultMinGasPrice = BigInteger.Parse(serviceSettings.DefaultMinGasPrice);
            _ethereum = ethereum;
            _gasPriceRepository = gasPriceRepository;
        }


        public async Task<BigInteger> CalculateGasPriceAsync(string to, BigInteger amount)
        {
            var estimatedGasPrice = await _ethereum.EstimateGasPriceAsync(to, amount);
            var minMaxGasPrice = await _gasPriceRepository.TryGetAsync();

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
