using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassic.Api.Actors.Extensions;
using Lykke.Service.EthereumClassic.Api.Actors.Messages;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Actors
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
                        operationId:  message.OperationId,
                        signedTxData: message.SignedTxData
                    );

                    // TODO: Add txHash to log

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

        private async Task ProcessMessageAsync(BuildTransaction message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var txData = await _transactionProcessorRole.BuildOperationAsync
                    (
                        amount:      message.Amount,
                        fromAddress: message.FromAddress,
                        includeFee:  message.IncludeFee,
                        operationId: message.OperationId,
                        toAddress:   message.ToAddress
                    );

                    Sender.Tell(new Status.Success
                    (
                        status: txData
                    ));
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

        private async Task ProcessMessageAsync(RebuildTransaction message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var txData = await _transactionProcessorRole.RebuildTransactionAsync
                    (
                        feeFactor:   message.FeeFactor,
                        operationId: message.OperationId
                    );

                    Sender.Tell(new Status.Success
                    (
                        status: txData
                    ));
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
    }
}
