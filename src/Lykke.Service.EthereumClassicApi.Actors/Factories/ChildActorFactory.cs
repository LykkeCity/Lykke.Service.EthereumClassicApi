using Akka.Actor;
using Akka.DI.Core;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Factories
{
    public abstract class ChildActorFactory<T> : IChildActorFactory
        where T : ActorBase
    {
        public virtual IActorRef Build(IUntypedActorContext context, string name)
        {
            return context.ActorOf
            (
                props: context.DI().Props<T>(),
                name:  name
            );
        }
    }
}