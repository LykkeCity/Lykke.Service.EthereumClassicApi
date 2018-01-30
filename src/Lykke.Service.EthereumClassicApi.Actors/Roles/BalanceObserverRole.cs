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
        private readonly IObservableBalanceRepository _observableBalanceRepository;


        public BalanceObserverRole(
            IEthereum ethereum,
            IObservableBalanceRepository observableBalanceRepository)
        {
            _ethereum = ethereum;
            _observableBalanceRepository = observableBalanceRepository;
        }


        public async Task<BigInteger> UpdateBalanceAsync(string address, BigInteger blockNumber)
        {
            var balance = await _observableBalanceRepository.TryGetAsync(address);

            if (balance != null && !balance.Locked)
            {
                var amount = await _ethereum.GetBalanceAsync(address, blockNumber);

                await _observableBalanceRepository.UpdateAmountAsync(address, amount);

                return amount;
            }
            
            return new BigInteger(0);
        }
    }
}
