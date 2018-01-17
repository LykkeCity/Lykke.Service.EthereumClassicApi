using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public class TransactionMonitorActor : ReceiveActor
    {
        private readonly ITransactionMonitorRole _transactionMonitorRole;

        public TransactionMonitorActor(
            ITransactionMonitorRole transactionMonitorRole)
        {
            _transactionMonitorRole = transactionMonitorRole;

            ReceiveAsync<DeleteTransactionState>(
                ProcessMessageAsync);

            ReceiveAsync<CheckTransactionState>(
                ProcessMessageAsync);
        }

        private async Task ProcessMessageAsync(CheckTransactionState message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var operationCompleted = await _transactionMonitorRole.CheckTransactionStateAsync(message.OperationId);

                    if (!operationCompleted)
                    {
                        ScheduleRetry(message);
                    }

                    logger.Suppress();
                }
                catch (Exception e)
                {
                    ScheduleRetry(message);

                    logger.Error(e);
                }
            }
        }

        private async Task ProcessMessageAsync(DeleteTransactionState message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    await _transactionMonitorRole.DeleteTransactionStateAsync(message.OperationId);

                    Sender.Tell(new Status.Success(null));
                }
                catch (Exception e)
                {
                    Sender.Tell(new Status.Failure
                    (
                        cause: e
                    ));

                    logger.Error(e);
                }
            }
        }

        private void ScheduleRetry(CheckTransactionState message)
        {
            Context.System.Scheduler.ScheduleTellOnce
            (
                delay:    TimeSpan.FromSeconds(30),
                receiver: Self,
                message:  message,
                sender:   Sender
            );
        }
    }
}
