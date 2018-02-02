using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Common.Chaos;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public class TransactionMonitorActor : ReceiveActor
    {
        private readonly ITransactionMonitorRole _transactionMonitorRole;
        private readonly IChaosKitty _chaosKitty;

        public TransactionMonitorActor(
            ITransactionMonitorRole transactionMonitorRole,
            IChaosKitty chaosKitty)
        {
            _transactionMonitorRole = transactionMonitorRole;
            _chaosKitty = chaosKitty;

            ReceiveAsync<CheckTransactionState>(
                ProcessMessageAsync);
        }

        private async Task ProcessMessageAsync(CheckTransactionState message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    _chaosKitty.Meow(message.OperationId);
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
