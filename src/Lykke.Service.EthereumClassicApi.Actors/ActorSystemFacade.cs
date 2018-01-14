using System;
using System.Collections.Generic;
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

        public ActorSystemFacade(IRootActorFactory rootActorFactory)
        {
            _rootActorFactory 
                = rootActorFactory;

            BalanceObserverDispatcher =
                _rootActorFactory.Build<BalanceObserverDispatcherActor>("balances-observer-dispatcher");

            BalanceObserverManager =
                _rootActorFactory.Build<BalanceObserverManagerActor>("balances-observer-manager");

            OperationMonitorDispatcher =
                _rootActorFactory.Build<OperationMonitorDispatcherActor>("operation-monitor-dispatcher");

            TransactionProcessorsDispatcher =
                _rootActorFactory.Build<TransactionProcessorDispatcherActor>("transaction-procesor-dispatcher");
        }


        public IActorRef BalanceObserverDispatcher { get; }

        public IActorRef BalanceObserverManager { get; }

        public IActorRef OperationMonitorDispatcher { get; }

        public IActorRef TransactionProcessorsDispatcher { get; }




        public async Task BeginBalanceMonitoringAsync(string address)
        {
            var response = await BalanceObserverManager.Ask(new BeginBalanceMonitoring
            (
                address: address
            ));

            CheckIfResponseIsSuccess(response);
        }

        public async Task BroadcastTransactionAsync(Guid operationId, string signedTxData)
        {
            var response = await TransactionProcessorsDispatcher.Ask(new BroadcastTransaction
            (
                operationId:  operationId,
                signedTxData: signedTxData
            ));

            CheckIfResponseIsSuccess(response);
        }

        public async Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress)
        {
            var response = await TransactionProcessorsDispatcher.Ask(new BuildTransaction
            (
                amount:      amount,
                fromAddress: fromAddress,
                includeFee:  includeFee,
                operationId: operationId,
                toAddress:   toAddress
            ));

            var txData = CheckIfResponseIs<string>(response);

            return txData;
        }

        public async Task DeleteOperationStateAsync(Guid operationId)
        {
            throw new NotImplementedException();
        }
        
        public async Task EndBalanceMonitoringAsync(string address)
        {
            var response = await BalanceObserverManager.Ask(new EndBalanceMonitoring
            (
                address: address
            ));

            CheckIfResponseIsSuccess(response);
        }

        public async Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId)
        {
            var response = await TransactionProcessorsDispatcher.Ask(new RebuildTransaction
            (
                feeFactor:   feeFactor,
                operationId: operationId
            ));

            var txData = CheckIfResponseIs<string>(response);

            return txData;
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
    }
}
