﻿using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Lykke.Service.EthereumClassic.Api.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Common.Settings;

namespace Lykke.Service.EthereumClassic.Api.Actors.Factories
{
    public class OperationMonitorsFactory : ChildActorFactory<OperationMonitorActor>, IOperationMonitorsFactory
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
                props: context.DI().Props<OperationMonitorActor>().WithRouter(router),
                name:  name
            );
        }
    }
}
