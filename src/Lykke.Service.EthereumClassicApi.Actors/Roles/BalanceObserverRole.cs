using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class BalanceObserverRole : IBalanceObserverRole
    {
        private readonly IObservableBalanceRepository _observableBalanceRepository;
        private readonly IEthereum                    _ethereum;


        public BalanceObserverRole(
            IObservableBalanceRepository observableBalanceRepository,
            IEthereum ethereum)
        {
            _observableBalanceRepository = observableBalanceRepository;
            _ethereum                    = ethereum;
        }


        public async Task<BigInteger> GetBalanceAsync(string address, BigInteger blockNumber)
        {
            var amount = await _ethereum.GetBalanceAsync(address, blockNumber);
            
            if (await _observableBalanceRepository.ExistsAsync(address))
            {
                var observableBalanceDto = new ObservableBalanceDto
                {
                    Address = address,
                    Amount  = amount
                };

                await _observableBalanceRepository.ReplaceAsync(observableBalanceDto);
            }

            return amount;
        }
    }
}
