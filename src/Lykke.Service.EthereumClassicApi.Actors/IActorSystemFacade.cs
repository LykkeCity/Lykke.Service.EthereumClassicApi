using System;
using System.Numerics;
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

        Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId,
            string toAddress);

        Task DeleteOperationStateAsync(Guid operationId);
        
        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);

        Task ShutdownAsync();
    }
}
