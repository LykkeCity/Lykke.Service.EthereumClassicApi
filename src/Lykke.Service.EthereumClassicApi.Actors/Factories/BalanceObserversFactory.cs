using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Common.Settings;

namespace Lykke.Service.EthereumClassicApi.Actors.Factories
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
            var router = new SmallestMailboxPool(_serviceSettings.NrOfBalanceObservers);

            return context.ActorOf
            (
                context.DI().Props<BalanceObserverActor>().WithRouter(router),
                name
            );
        }
    }
}
