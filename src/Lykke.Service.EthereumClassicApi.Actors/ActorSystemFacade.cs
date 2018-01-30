using System;
using System.Numerics;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Common.Exceptions;

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
                _rootActorFactory.Build<BalanceObserverDispatcherActor>("balances-observers-dispatcher");
            
            TransactionMonitorDispatcher =
                _rootActorFactory.Build<TransactionMonitorDispatcherActor>("transaction-monitors-dispatcher");

            TransactionBroadcastersDispatcher =
                _rootActorFactory.Build<TransactionBroadcastersDispatcherActor>("transaction-broadcasters-dispatcher");
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

        public IActorRef TransactionBroadcastersDispatcher { get; }


        
        public async Task BroadcastTransactionAsync(Guid operationId, string signedTxData)
        {
            var response = await TransactionBroadcastersDispatcher.Ask(new BroadcastTransaction
            (
                operationId,
                signedTxData
            ));

            CheckIfResponseIsSuccess(response);
        }
        
        public async Task ShutdownAsync()
        {
            var gracefulStutdownPeriod = TimeSpan.FromMinutes(1);

            await Task.WhenAll
            (
                BalanceObserverDispatcher.GracefulStop(gracefulStutdownPeriod),
                TransactionMonitorDispatcher.GracefulStop(gracefulStutdownPeriod),
                TransactionBroadcastersDispatcher.GracefulStop(gracefulStutdownPeriod)
            );
            
            await _shutdownCallback();
        }
    }
}
