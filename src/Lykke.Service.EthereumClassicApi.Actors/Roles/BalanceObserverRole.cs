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
        private readonly IEthereum _ethereum;
        private readonly IObservableBalanceLockRepository _observableBalanceLockRepository;
        private readonly IObservableBalanceRepository _observableBalanceRepository;


        public BalanceObserverRole(
            IEthereum ethereum,
            IObservableBalanceLockRepository observableBalanceLockRepository,
            IObservableBalanceRepository observableBalanceRepository)
        {
            _ethereum = ethereum;
            _observableBalanceRepository = observableBalanceRepository;
            _observableBalanceLockRepository = observableBalanceLockRepository;
        }


        public async Task<BigInteger> GetBalanceAsync(string address, BigInteger blockNumber)
        {
            var amount = !await _observableBalanceLockRepository.ExistsAsync(address)
                ? await _ethereum.GetBalanceAsync(address, blockNumber)
                : new BigInteger(0);

            if (await _observableBalanceRepository.ExistsAsync(address))
            {
                var observableBalanceDto = new ObservableBalanceDto
                {
                    Address = address,
                    Amount = amount
                };

                await _observableBalanceRepository.ReplaceAsync(observableBalanceDto);
            }

            return amount;
        }
    }
}
