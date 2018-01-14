using System;
using System.Numerics;
using System.Threading.Tasks;
using Akka.Actor;


namespace Lykke.Service.EthereumClassicApi.Actors
{
    public interface IActorSystemFacade
    {
        IActorRef BalanceObserverDispatcher { get; }

        IActorRef BalanceObserverManager { get; }

        IActorRef OperationMonitorDispatcher { get; }

        IActorRef TransactionProcessorsDispatcher { get; }
        

        Task BeginBalanceMonitoringAsync(string address);

        Task BroadcastTransactionAsync(Guid operationId, string signedTxData);

        Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress);

        Task DeleteOperationStateAsync(Guid operationId);

        Task EndBalanceMonitoringAsync(string address);

        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);
    }
}
