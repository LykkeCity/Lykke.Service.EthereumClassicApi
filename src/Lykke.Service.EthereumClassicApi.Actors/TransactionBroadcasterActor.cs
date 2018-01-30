using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public class TransactionBroadcasterActor : ReceiveActor
    {
        private readonly ITransactionBroadcasterRole _transactionBroadcasterRole;


        public TransactionBroadcasterActor(
            ITransactionBroadcasterRole transactionBroadcasterRole)
        {
            _transactionBroadcasterRole = transactionBroadcasterRole;


            ReceiveAsync<BroadcastTransaction>(
                ProcessMessageAsync);
        }


        private async Task ProcessMessageAsync(BroadcastTransaction message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var txHash = await _transactionBroadcasterRole.BroadcastTransaction
                    (
                        message.OperationId,
                        message.SignedTxData
                    );

                    Context.System.EventStream.Publish(new TransactionBroadcasted
                    (
                        message.OperationId
                    ));


                    logger.Info($"Transaction [{txHash}] has been broadcasted");

                    Sender.Tell(new Status.Success(null));
                }
                catch (Exception e)
                {
                    Sender.Tell(new Status.Failure
                    (
                        e
                    ));

                    logger.Error(e);
                }
            }
        }
    }
}
