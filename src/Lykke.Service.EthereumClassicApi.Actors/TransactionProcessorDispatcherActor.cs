using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Common.Utils;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public class TransactionProcessorDispatcherActor : ReceiveActor
    {
        private readonly ITransactionProcessorDispatcherRole _transactionProcessorDispatcherRole;
        private readonly ITransactionProcessorFactory _transactionProcessorFactory;
        private readonly Dictionary<string, TransactionProcessorRef> _transactionProcessors;


        public TransactionProcessorDispatcherActor(
            ITransactionProcessorDispatcherRole transactionProcessorDispatcherRole,
            ITransactionProcessorFactory transactionProcessorFactory)
        {
            _transactionProcessorDispatcherRole = transactionProcessorDispatcherRole;
            _transactionProcessorFactory = transactionProcessorFactory;
            _transactionProcessors = new Dictionary<string, TransactionProcessorRef>();

            
            Receive<CleanupExpiredTransactionProcessors>(
                msg => ProcessMessage(msg));

            ReceiveAsync<BroadcastTransaction>(
                ProcessMessageAsync);
            
        }


        private void ProcessMessage(CleanupExpiredTransactionProcessors message)
        {
            using (var logger = Context.GetLogger(message))
            {
                var expirationTime = UtcNow.Get().Subtract(TimeSpan.FromDays(1));

                try
                {
                    _transactionProcessors
                        .Where(x => x.Value.LastTalkTime <= expirationTime)
                        .ToList()
                        .ForEach(x =>
                        {
                            x.Value.Tell(PoisonPill.Instance);

                            _transactionProcessors.Remove(x.Key);
                        });
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
        }

        private async Task ProcessMessageAsync(BroadcastTransaction message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var fromAddress =
                        await _transactionProcessorDispatcherRole.GetFromAddressAsync(message.OperationId);
                    var processor = GetOrCreateTransactionProcessor(fromAddress);

                    processor.Forward(message);
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

        #region Common

        private IActorRef GetOrCreateTransactionProcessor(string address)
        {
            if (!_transactionProcessors.ContainsKey(address))
            {
                _transactionProcessors[address] = new TransactionProcessorRef
                (
                    _transactionProcessorFactory.Build(Context, $"transaction-processor-{address}")
                );
            }

            return _transactionProcessors[address];
        }

        private class TransactionProcessorRef : IActorRef
        {
            private readonly IActorRef _actorRef;


            public TransactionProcessorRef(IActorRef actorRef)
            {
                _actorRef = actorRef;
            }

            public DateTime LastTalkTime { get; private set; }

            public void Tell(object message, IActorRef sender)
            {
                LastTalkTime = UtcNow.Get();

                _actorRef.Tell(message, sender);
            }

            public int CompareTo(IActorRef other)
            {
                return _actorRef.CompareTo(other);
            }

            public int CompareTo(object obj)
            {
                return _actorRef.CompareTo(obj);
            }

            public bool Equals(IActorRef other)
            {
                return _actorRef.Equals(other);
            }

            public ActorPath Path
                => _actorRef.Path;

            public ISurrogate ToSurrogate(ActorSystem system)
            {
                return _actorRef.ToSurrogate(system);
            }
        }

        #endregion
    }
}
