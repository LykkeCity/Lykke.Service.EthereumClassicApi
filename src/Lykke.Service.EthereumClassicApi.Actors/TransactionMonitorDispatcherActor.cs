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
    public class TransactionMonitorDispatcherActor : ReceiveActor
    {
        private readonly EthereumClassicApiSettings _settings;
        private readonly ITransactionMonitorDispatcherRole _transactionMonitorDispatcherRole;
        private readonly IActorRef _transactionMonitors;

        private int _numberOfRemainingTransactions;


        public TransactionMonitorDispatcherActor(
            IOperationMonitorsFactory operationMonitorsFactory,
            EthereumClassicApiSettings settings,
            ITransactionMonitorDispatcherRole transactionMonitorDispatcherRole)
        {
            _transactionMonitorDispatcherRole = transactionMonitorDispatcherRole;
            _transactionMonitors = operationMonitorsFactory.Build(Context, "transation-monitors");
            _settings = settings;

            Become(Idle);
            
            Self.Tell
            (
                new CheckTransactionStates(),
                Nobody.Instance
            );
        }

        #region Busy state

        private void Busy()
        {
            Receive<TransactionStateChecked>(
                msg => ProcessMessageWhenBusy(msg));

            Receive<CheckTransactionStates>(
                msg => { });
        }

        private void ProcessMessageWhenBusy(TransactionStateChecked message)
        {
            if (--_numberOfRemainingTransactions == 0)
            {
                Become(Idle);

                ScheduleTransactionStatesCheck();
            }
        }

        #endregion

        #region Idle state

        private void Idle()
        {
            ReceiveAsync<CheckTransactionStates>(
                ProcessMessageWhenIdleAsync);
        }

        private async Task ProcessMessageWhenIdleAsync(CheckTransactionStates message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var operationIds = (await _transactionMonitorDispatcherRole.GetAllInProgressOperationIdsAsync()).ToList();

                    foreach (var operationId in operationIds)
                    {
                        _transactionMonitors.Tell(new CheckTransactionState
                        (
                            operationId
                        ));
                    }

                    if (operationIds.Count > 0)
                    {
                        _numberOfRemainingTransactions = operationIds.Count;

                        Become(Busy);
                    }
                    else
                    {
                        ScheduleTransactionStatesCheck();
                    }
                }
                catch (Exception e)
                {
                    ScheduleTransactionStatesCheck();

                    logger.Error(e);
                }
            }
        }

        #endregion

        #region Common

        private void ScheduleTransactionStatesCheck()
        {
            Context.System.Scheduler.ScheduleTellOnce
            (
                delay: _settings.TransactionStatesCheckInterval,
                receiver: Self,
                message: new CheckTransactionStates(),
                sender: Nobody.Instance
            );
        }

        #endregion
    }
}
