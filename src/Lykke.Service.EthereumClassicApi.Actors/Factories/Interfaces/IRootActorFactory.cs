using Akka.Actor;

namespace Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces
{
    public interface IRootActorFactory
    {
        IActorRef Build<T>(string name)
            where T : ActorBase;
    }
}
