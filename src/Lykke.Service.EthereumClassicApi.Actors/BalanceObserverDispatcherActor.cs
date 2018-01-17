using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Actors
{
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public class BalanceObserverDispatcherActor : ReceiveActor
    { 
        private readonly IActorRef                      _balanceReaders;
        private readonly IBalanceObserverDispatcherRole _balanceObserverDispatcherRole;

        private int _numberOfRemainingBalances;


        public BalanceObserverDispatcherActor(
            IBalanceObserverDispatcherRole balanceObserverDispatcherRole,
            IBalanceObserversFactory balanceObserversFactory)
        {
            _balanceObserverDispatcherRole = balanceObserverDispatcherRole;
            _balanceReaders                = balanceObserversFactory.Build(Context, "balance-readers");


            Become(Idle);
        }


        #region Busy state

        private void Busy()
        {
            Receive<BalanceChecked>(
                msg => ProcessMessageWhenBusy(msg));
        }

        private void ProcessMessageWhenBusy(BalanceChecked message)
        {
            if (--_numberOfRemainingBalances == 0)
            {
                Become(Idle);
            }
        }

        #endregion

        #region Idle state

        public void Idle()
        {
            ReceiveAsync<CheckBalances>(
                ProcessMessageWhenIdleAsync);
        }

        private async Task ProcessMessageWhenIdleAsync(CheckBalances message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var observableAddresses        = (await _balanceObserverDispatcherRole.GetObservableAddressesAsync()).ToList();
                    var latestConfirmedBlockNumber = await _balanceObserverDispatcherRole.GetLatestConfirmedBlockNumber();

                    foreach (var address in observableAddresses)
                    {
                        _balanceReaders.Tell(new CheckBalance
                        (
                            address:      address,
                            blockNumbber: latestConfirmedBlockNumber
                        ));
                    }
                    
                    if (observableAddresses.Count > 0)
                    {
                        _numberOfRemainingBalances = observableAddresses.Count;

                        Become(Busy);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
        }

        #endregion
    }
}
