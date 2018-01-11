using Akka.Actor;
using Akka.DI.Core;
using Lykke.Service.EthereumClassic.Api.Actors.Factories.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Actors.Factories
{
    public class RootActorFactory : IRootActorFactory
    {
        private readonly ActorSystem _actorSystem;


        public RootActorFactory(
            ActorSystem actorSystem)
        {
            _actorSystem = actorSystem;
        }


        public IActorRef Build<T>(string name)
            where T : ActorBase
        {
            return _actorSystem.ActorOf
            (
                props: _actorSystem.DI().Props<T>(),
                name:  name
            );
        }
    }
}