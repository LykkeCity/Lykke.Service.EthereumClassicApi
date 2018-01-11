using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassic.Api.Blockchain.Interfaces;
using Lykke.Service.EthereumClassic.Api.Common.Settings;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;


namespace Lykke.Service.EthereumClassic.Api.Actors.Roles
{
    public class BalanceObserverDispatcherRole : IBalanceObserverDispatcherRole
    {
        private readonly IEthereum                   _ethereum;
        private readonly IObservableBalanceRepository _observableBalanceRepository;
        private readonly EthereumClassicApiSettings  _settings;


        public BalanceObserverDispatcherRole(
            IEthereum ethereum,
            IObservableBalanceRepository observableBalanceRepository,
            EthereumClassicApiSettings settings)
        {
            _ethereum                   = ethereum;
            _observableBalanceRepository = observableBalanceRepository;
            _settings                   = settings;
        }

        [Pure]
        public async Task<IEnumerable<string>> GetObservableAddressesAsync()
        {
            return (await _observableBalanceRepository.GetAllAsync())
                .Select(x => x.Address);
        }

        [Pure]
        public async Task<BigInteger> GetLatestConfirmedBlockNumber()
        {
            var latestBlockNumber          = await _ethereum.GetLatestBlockNumberAsync();
            var latestConfirmedBlockNumber = latestBlockNumber - _settings.TransactionConfirmationLevel;

            return latestConfirmedBlockNumber;
        }
    }
}
