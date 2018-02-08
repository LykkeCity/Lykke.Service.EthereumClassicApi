using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Common.Settings;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public class BalanceObserverDispatcherActor : ReceiveActor
    {
        private readonly IBalanceObserverDispatcherRole _balanceObserverDispatcherRole;
        private readonly IActorRef _balanceObservers;
        private readonly EthereumClassicApiSettings _settings;

        private int _numberOfRemainingBalances;


        public BalanceObserverDispatcherActor(
            IBalanceObserverDispatcherRole balanceObserverDispatcherRole,
            IBalanceObserversFactory balanceObserversFactory,
            EthereumClassicApiSettings settings)
        {
            _balanceObserverDispatcherRole = balanceObserverDispatcherRole;
            _balanceObservers = balanceObserversFactory.Build(Context, "balance-observers");
            _settings = settings;

            Become(Idle);

            Self.Tell
            (
                new CheckBalances(),
                Nobody.Instance
            );
        }


        #region Busy state

        private void Busy()
        {
            Receive<BalanceChecked>(
                msg => ProcessMessageWhenBusy(msg));

            Receive<CheckBalances>(
                msg => { });
        }

        private void ProcessMessageWhenBusy(BalanceChecked message)
        {
            if (--_numberOfRemainingBalances == 0)
            {
                Become(Idle);

                ScheduleBalancesCheck();
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
                    var observableAddresses = (await _balanceObserverDispatcherRole.GetObservableAddressesAsync()).ToList();
                    var latestConfirmedBlockNumber = await _balanceObserverDispatcherRole.GetLatestConfirmedBlockNumber();

                    foreach (var address in observableAddresses)
                    {
                        _balanceObservers.Tell(new CheckBalance
                        (
                            address,
                            latestConfirmedBlockNumber
                        ));
                    }

                    if (observableAddresses.Count > 0)
                    {
                        _numberOfRemainingBalances = observableAddresses.Count;

                        Become(Busy);
                    }
                    else
                    {
                        ScheduleBalancesCheck();
                    }
                }
                catch (Exception e)
                {
                    ScheduleBalancesCheck();

                    logger.Error(e);
                }
            }
        }

        #endregion

        #region Common

        private void ScheduleBalancesCheck()
        {
            Context.System.Scheduler.ScheduleTellOnce
            (
                delay: _settings.BalancesCheckInterval,
                receiver: Self,
                message: new CheckBalances(),
                sender: Nobody.Instance
            );
        }

        #endregion
    }
}
