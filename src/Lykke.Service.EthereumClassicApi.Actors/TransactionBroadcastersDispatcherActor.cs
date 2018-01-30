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
    public class TransactionBroadcastersDispatcherActor : ReceiveActor
    {
        private readonly ITransactionBroadcasterDispatcherRole _transactionBroadcasterDispatcherRole;
        private readonly ITransactionBroadcasterFactory _transactionBroadcasterFactory;
        private readonly Dictionary<string, TransactionBroadcasterRef> _transactionBroadcasters;


        public TransactionBroadcastersDispatcherActor(
            ITransactionBroadcasterDispatcherRole transactionBroadcasterDispatcherRole,
            ITransactionBroadcasterFactory transactionBroadcasterFactory)
        {
            _transactionBroadcasterDispatcherRole = transactionBroadcasterDispatcherRole;
            _transactionBroadcasterFactory = transactionBroadcasterFactory;
            _transactionBroadcasters = new Dictionary<string, TransactionBroadcasterRef>();

            
            Receive<CleanupExpiredTransactionBroadcasters>(
                msg => ProcessMessage(msg));

            ReceiveAsync<BroadcastTransaction>(
                ProcessMessageAsync);
            
        }


        private void ProcessMessage(CleanupExpiredTransactionBroadcasters message)
        {
            using (var logger = Context.GetLogger(message))
            {
                var expirationTime = UtcNow.Get().Subtract(TimeSpan.FromDays(1));

                try
                {
                    _transactionBroadcasters
                        .Where(x => x.Value.LastTalkTime <= expirationTime)
                        .ToList()
                        .ForEach(x =>
                        {
                            x.Value.Tell(PoisonPill.Instance);

                            _transactionBroadcasters.Remove(x.Key);
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
                    var fromAddress = await _transactionBroadcasterDispatcherRole.GetFromAddressAsync(message.OperationId);
                    var processor = GetOrCreateTransactionBroadcaster(fromAddress);

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

        private IActorRef GetOrCreateTransactionBroadcaster(string address)
        {
            if (!_transactionBroadcasters.ContainsKey(address))
            {
                _transactionBroadcasters[address] = new TransactionBroadcasterRef
                (
                    _transactionBroadcasterFactory.Build(Context, $"transaction-broadcaster-{address}-{Guid.NewGuid():N}")
                );
            }

            return _transactionBroadcasters[address];
        }

        private class TransactionBroadcasterRef : IActorRef
        {
            private readonly IActorRef _actorRef;


            public TransactionBroadcasterRef(IActorRef actorRef)
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
