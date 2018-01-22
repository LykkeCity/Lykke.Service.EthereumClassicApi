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
        private readonly IBroadcastedTransactionStateRepository _broadcastedTransactionStateRepository;
        private readonly IEthereum                              _ethereum;
        private readonly IObservableBalanceRepository           _observableBalanceRepository;
        
        
        public BalanceObserverRole(
            IBroadcastedTransactionStateRepository broadcastedTransactionStateRepository,
            IEthereum ethereum,
            IObservableBalanceRepository observableBalanceRepository)
        {
            _broadcastedTransactionStateRepository = broadcastedTransactionStateRepository;
            _ethereum                              = ethereum;
            _observableBalanceRepository           = observableBalanceRepository;
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
