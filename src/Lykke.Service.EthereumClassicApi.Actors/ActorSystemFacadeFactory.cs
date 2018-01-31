using Akka.Actor;
using Akka.Configuration;
using Autofac;
using Common.Log;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Factories;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Logger;
using Lykke.SlackNotifications;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public sealed class ActorSystemFacadeFactory
    {
        private readonly ActorSystem _actorSystem;
        private readonly IRootActorFactory _rootActorFactory;


        internal ActorSystemFacadeFactory(
            ActorSystem actorSystem,
            IRootActorFactory rootActorFactory)
        {
            _actorSystem = actorSystem;
            _rootActorFactory = rootActorFactory;
        }

        public static IActorSystemFacade Build(IContainer container)
        {
            var actorSystem = BuildActorSystem(container);
            var actorFactory = new RootActorFactory(actorSystem);
            var facadeFactory = new ActorSystemFacadeFactory(actorSystem, actorFactory);
            
            return facadeFactory.Build();
        }

        internal IActorSystemFacade Build()
        {
            var facade = new ActorSystemFacade
            (
                rootActorFactory: _rootActorFactory,
                shutdownCallback: async () => await CoordinatedShutdown.Get(_actorSystem).Run()
            );
            
            return facade;
        }

        private static ActorSystem BuildActorSystem(IContainer container)
        {
            var log = container.Resolve<ILog>();
            var notificationSender = container.Resolve<ISlackNotificationsSender>();

            LykkeLogger.Configure(log, notificationSender);

            var systemConfig = ConfigurationFactory.FromResource
            (
                "Lykke.Service.EthereumClassicApi.Actors.SystemConfig.json",
                typeof(ActorSystemFacadeFactory).Assembly
            );
            
            return ActorSystem
                .Create("ethereum-classic", systemConfig)
                .WithContainer(container);
        }
    }
}
