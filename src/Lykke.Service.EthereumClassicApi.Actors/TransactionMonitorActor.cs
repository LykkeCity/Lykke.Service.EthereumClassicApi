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
                    var transactionCompleted = await _transactionMonitorRole.CheckTransactionStatesAsync(message.OperationId);

                    if (transactionCompleted)
                    {
                        logger.Info($"Operation [{message.OperationId}] completed.");
                    }
                    else
                    {
                        logger.Suppress();
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
                finally
                {
                    Sender.Tell(TransactionStateChecked.Instance);
                }
            }
        }
    }
}
