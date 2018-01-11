using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Lykke.Service.EthereumClassic.Api.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Common.Settings;

namespace Lykke.Service.EthereumClassic.Api.Actors.Factories
{
    public class BalanceObserversFactory : ChildActorFactory<BalanceObserverActor>, IBalanceObserversFactory
    {
        private readonly EthereumClassicApiSettings _serviceSettings;

        public BalanceObserversFactory(
            EthereumClassicApiSettings serviceSettings)
        {
            _serviceSettings = serviceSettings;
        }

        
        public override IActorRef Build(IUntypedActorContext context, string name)
        {
            var router = new SmallestMailboxPool(_serviceSettings.NrOfBalanceReaders);

            return context.ActorOf
            (
                props: context.DI().Props<BalanceObserverActor>().WithRouter(router),
                name:  name
            );
        }
    }
}
