using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Akka.Actor;


namespace Lykke.Service.EthereumClassic.Api.Actors
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

        Task DeleteOperationStates(IEnumerable<Guid> operationIds);

        Task EndBalanceMonitoringAsync(string address);

        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);
    }
}
