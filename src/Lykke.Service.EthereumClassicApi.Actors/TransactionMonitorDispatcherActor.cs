using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public class TransactionMonitorDispatcherActor : ReceiveActor
    {
        private readonly ITransactionMonitorDispatcherRole _transactionMonitorDispatcherRole;
        private readonly IActorRef                         _transactionMonitors;


        public TransactionMonitorDispatcherActor(
            ITransactionMonitorDispatcherRole transactionMonitorDispatcherRole,
            IOperationMonitorsFactory operationMonitorsFactory)
        {
            _transactionMonitorDispatcherRole = transactionMonitorDispatcherRole;
            _transactionMonitors              = operationMonitorsFactory.Build(Context, "transation-monitors");


            Context.System.EventStream
                .Subscribe(Self, typeof(TransactionBroadcasted));


            ReceiveAsync<CheckTransactionStates>(
                ProcessMessageAsync);

            Receive<DeleteTransactionState>(
                msg => ProcessMessage(msg));

            Receive<TransactionBroadcasted>(
                msg => ProcessMessage(msg));


            Self.Tell
            (
                message: CheckTransactionStates.Instance,
                sender:  Nobody.Instance
            );
        }
        

        private void ProcessMessage(DeleteTransactionState message)
        {
            _transactionMonitors.Forward(message);
        }

        private async Task ProcessMessageAsync(CheckTransactionStates message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var operationIds = await _transactionMonitorDispatcherRole.GetAllOperationIdsAsync();

                    foreach (var operationId in operationIds)
                    {
                        _transactionMonitors.Tell(new CheckTransactionState
                        (
                            operationId: operationId
                        ));
                    }
                }
                catch (Exception e)
                {
                    Context.System.Scheduler.ScheduleTellOnce
                    (
                        delay:    TimeSpan.FromMinutes(1),
                        receiver: Self,
                        message:  message,
                        sender:   Nobody.Instance
                    );

                    logger.Error(e);
                }
            }
        }

        private void ProcessMessage(TransactionBroadcasted message)
        {
            _transactionMonitors.Tell(new CheckTransactionState
            (
                operationId: message.OperationId
            ));
        }
    }
}
