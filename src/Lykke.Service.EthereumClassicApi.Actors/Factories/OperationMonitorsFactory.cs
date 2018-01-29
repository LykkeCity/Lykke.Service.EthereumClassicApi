using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Common.Settings;

namespace Lykke.Service.EthereumClassicApi.Actors.Factories
{
    public class OperationMonitorsFactory : ChildActorFactory<TransactionMonitorActor>, IOperationMonitorsFactory
    {
        private readonly EthereumClassicApiSettings _serviceSettings;

        public OperationMonitorsFactory(
            EthereumClassicApiSettings serviceSettings)
        {
            _serviceSettings = serviceSettings;
        }


        public override IActorRef Build(IUntypedActorContext context, string name)
        {
            var router = new SmallestMailboxPool(_serviceSettings.NrOfOperationMonitors);

            return context.ActorOf
            (
                context.DI().Props<TransactionMonitorActor>().WithRouter(router),
                name
            );
        }
    }
}
