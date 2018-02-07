using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class BalanceObserverDispatcherRole : IBalanceObserverDispatcherRole
    {
        private readonly IEthereum _ethereum;
        private readonly IObservableBalanceRepository _observableBalanceRepository;
        private readonly EthereumClassicApiSettings _settings;


        public BalanceObserverDispatcherRole(
            IEthereum ethereum,
            IObservableBalanceRepository observableBalanceRepository,
            EthereumClassicApiSettings settings)
        {
            _ethereum = ethereum;
            _observableBalanceRepository = observableBalanceRepository;
            _settings = settings;
        }

        [Pure]
        public async Task<IEnumerable<string>> GetObservableAddressesAsync()
        {
            string continuationToken = null;
            var addresses = new List<string>();

            do
            {
                IEnumerable<ObservableBalanceEntity> balances;

                (balances, continuationToken) = (await _observableBalanceRepository.GetAllAsync(1000, continuationToken));

                addresses.AddRange(balances.Select(x => x.Address));

            } while (continuationToken != null);

            return addresses;
        }

        [Pure]
        public async Task<BigInteger> GetLatestConfirmedBlockNumber()
        {
            var latestBlockNumber = await _ethereum.GetLatestBlockNumberAsync();
            var latestConfirmedBlockNumber = latestBlockNumber - _settings.TransactionConfirmationLevel;

            return latestConfirmedBlockNumber;
        }
    }
}
