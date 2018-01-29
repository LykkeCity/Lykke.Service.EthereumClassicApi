using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public class TransactionProcessorActor : ReceiveActor
    {
        private readonly ITransactionProcessorRole _transactionProcessorRole;


        public TransactionProcessorActor(
            ITransactionProcessorRole transactionProcessorRole)
        {
            _transactionProcessorRole = transactionProcessorRole;


            ReceiveAsync<BroadcastTransaction>(
                ProcessMessageAsync);

            ReceiveAsync<BuildTransaction>(
                ProcessMessageAsync);

            ReceiveAsync<RebuildTransaction>(
                ProcessMessageAsync);
        }


        private async Task ProcessMessageAsync(BroadcastTransaction message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var txHash = await _transactionProcessorRole.BroadcastTransaction
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

        private async Task ProcessMessageAsync(BuildTransaction message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var txData = await _transactionProcessorRole.BuildTransactionAsync
                    (
                        message.Amount,
                        message.FromAddress,
                        message.IncludeFee,
                        message.OperationId,
                        message.ToAddress
                    );

                    Sender.Tell(new TransactionBuilt
                    (
                        txData
                    ));
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

        private async Task ProcessMessageAsync(RebuildTransaction message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var txData = await _transactionProcessorRole.RebuildTransactionAsync
                    (
                        message.FeeFactor,
                        message.OperationId
                    );

                    Sender.Tell(new TransactionBuilt
                    (
                        txData
                    ));
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
