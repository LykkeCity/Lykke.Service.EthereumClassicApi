using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors.Messages;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public class ActorSystemFacade : IActorSystemFacade
    {
        private readonly Func<Task> _shutdownCallback;

        public ActorSystemFacade(
            IRootActorFactory rootActorFactory,
            Func<Task> shutdownCallback)
        {
            _shutdownCallback
                = shutdownCallback;

            BalanceObserverDispatcher =
                rootActorFactory.Build<BalanceObserverDispatcherActor>("balances-observers-dispatcher");
            
            TransactionMonitorDispatcher =
                rootActorFactory.Build<TransactionMonitorDispatcherActor>("transaction-monitors-dispatcher");
        }


        public IActorRef BalanceObserverDispatcher { get; }
        
        public IActorRef TransactionMonitorDispatcher { get; }


        public void OnTransactionBroadcasted(Guid operationId)
        {
            TransactionMonitorDispatcher.Tell(new CheckTransactionState
            (
                operationId: operationId
            ));
        }

        public async Task ShutdownAsync()
        {
            var gracefulStutdownPeriod = TimeSpan.FromMinutes(1);

            await Task.WhenAll
            (
                BalanceObserverDispatcher.GracefulStop(gracefulStutdownPeriod),
                TransactionMonitorDispatcher.GracefulStop(gracefulStutdownPeriod)
            );
            
            await _shutdownCallback();
        }
    }
}
