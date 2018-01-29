using System;
using System.Numerics;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Exceptions;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors.Messages;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public class ActorSystemFacade : IActorSystemFacade
    {
        private readonly IRootActorFactory _rootActorFactory;
        private readonly Func<Task> _shutdownCallback;

        public ActorSystemFacade(
            IRootActorFactory rootActorFactory,
            Func<Task> shutdownCallback)
        {
            _rootActorFactory
                = rootActorFactory;

            _shutdownCallback
                = shutdownCallback;

            BalanceObserverDispatcher =
                _rootActorFactory.Build<BalanceObserverDispatcherActor>("balances-observer-dispatcher");
            
            TransactionMonitorDispatcher =
                _rootActorFactory.Build<TransactionMonitorDispatcherActor>("transaction-monitor-dispatcher");

            TransactionProcessorsDispatcher =
                _rootActorFactory.Build<TransactionProcessorDispatcherActor>("transaction-procesor-dispatcher");
        }

        private static T CheckIfResponseIs<T>(object response)
        {
            switch (response)
            {
                case T expectedResponse:
                    return expectedResponse;
                case Status.Failure failure:
                    throw failure.Cause;
                default:
                    throw new UnexpectedResponseException(response);
            }
        }

        private static void CheckIfResponseIsSuccess(object response)
        {
            CheckIfResponseIs<Status.Success>(response);
        }


        public IActorRef BalanceObserverDispatcher { get; }
        
        public IActorRef TransactionMonitorDispatcher { get; }

        public IActorRef TransactionProcessorsDispatcher { get; }


        
        public async Task BroadcastTransactionAsync(Guid operationId, string signedTxData)
        {
            var response = await TransactionProcessorsDispatcher.Ask(new BroadcastTransaction
            (
                operationId,
                signedTxData
            ));

            CheckIfResponseIsSuccess(response);
        }

        public async Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee,
            Guid operationId, string toAddress)
        {
            var response = await TransactionProcessorsDispatcher.Ask(new BuildTransaction
            (
                amount,
                fromAddress,
                includeFee,
                operationId,
                toAddress
            ));

            var txData = CheckIfResponseIs<TransactionBuilt>(response).TxData;

            return txData;
        }

        public async Task DeleteOperationStateAsync(Guid operationId)
        {
            var response = await TransactionMonitorDispatcher.Ask(new DeleteTransactionState
            (
                operationId
            ));

            CheckIfResponseIsSuccess(response);
        }
        
        public async Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId)
        {
            var response = await TransactionProcessorsDispatcher.Ask(new RebuildTransaction
            (
                feeFactor,
                operationId
            ));

            var txData = CheckIfResponseIs<TransactionBuilt>(response).TxData;

            return txData;
        }

        public async Task ShutdownAsync()
        {
            var gracefulStutdownPeriod = TimeSpan.FromMinutes(1);

            await Task.WhenAll
            (
                BalanceObserverDispatcher.GracefulStop(gracefulStutdownPeriod),
                TransactionMonitorDispatcher.GracefulStop(gracefulStutdownPeriod),
                TransactionProcessorsDispatcher.GracefulStop(gracefulStutdownPeriod)
            );
            
            await _shutdownCallback();
        }
    }
}
