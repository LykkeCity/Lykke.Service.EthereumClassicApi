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
            
            ReceiveAsync<CheckTransactionState>(
                ProcessMessageAsync);
        }

        private async Task ProcessMessageAsync(CheckTransactionState message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var operationCompleted =
                        await _transactionMonitorRole.CheckTransactionStatesAsync(message.OperationId);

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

        private void ScheduleRetry(CheckTransactionState message)
        {
            Context.System.Scheduler.ScheduleTellOnce
            (
                TimeSpan.FromSeconds(30),
                Self,
                message,
                Sender
            );
        }
    }
}
