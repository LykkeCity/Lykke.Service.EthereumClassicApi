using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public interface IActorSystemFacade
    {
        IActorRef BalanceObserverDispatcher { get; }
        
        IActorRef TransactionMonitorDispatcher { get; }

        IActorRef TransactionProcessorsDispatcher { get; }
        

        Task BroadcastTransactionAsync(Guid operationId, string signedTxData);
        
        Task ShutdownAsync();
    }
}
