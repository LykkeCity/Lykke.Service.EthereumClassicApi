using Akka.Actor;

namespace Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces
{
    public interface IChildActorFactory
    {
        IActorRef Build(IUntypedActorContext context, string name);
    }
}
