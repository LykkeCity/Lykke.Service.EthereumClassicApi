using Akka.Actor;

namespace Lykke.Service.EthereumClassic.Api.Actors.Factories.Interfaces
{
    public interface IChildActorFactory
    {
        IActorRef Build(IUntypedActorContext context, string name);
    }
}