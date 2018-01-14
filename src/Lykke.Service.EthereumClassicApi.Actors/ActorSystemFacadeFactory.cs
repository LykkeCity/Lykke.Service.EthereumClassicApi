using Akka.Actor;
using Akka.Configuration;
using Autofac;
using Common.Log;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Factories;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Logger;
using Lykke.SlackNotifications;


namespace Lykke.Service.EthereumClassicApi.Actors
{
    public sealed class ActorSystemFacadeFactory
    {
        private readonly IRootActorFactory          _rootActorFactory;
        private readonly EthereumClassicApiSettings _serviceSettings;
        private readonly IScheduler                 _scheduler;


        internal ActorSystemFacadeFactory(
            IRootActorFactory rootActorFactory,
            EthereumClassicApiSettings serviceSettings,
            IScheduler scheduler)
        {
            _rootActorFactory = rootActorFactory;
            _serviceSettings  = serviceSettings;
            _scheduler        = scheduler;
        }

        internal IActorSystemFacade Build()
        {
            var facade = new ActorSystemFacade(_rootActorFactory);
            
            _scheduler.ScheduleTellRepeatedly
            (
                initialDelay: _serviceSettings.BalancesCheckInterval,
                interval:     _serviceSettings.BalancesCheckInterval,
                receiver:     facade.BalanceObserverDispatcher,
                message:      CheckBalances.Instance,
                sender:       Nobody.Instance
            );

            return facade;
        }

        public static IActorSystemFacade Build(IContainer container)
        {
            var actorSystem   = BuildActorSystem(container);
            var actorFactory  = new RootActorFactory(actorSystem);
            var settings      = container.Resolve<EthereumClassicApiSettings>();
            var facadeFactory = new ActorSystemFacadeFactory(actorFactory, settings, actorSystem.Scheduler);
            

            return facadeFactory.Build();
        }

        private static ActorSystem BuildActorSystem(IContainer container)
        {
            var log                = container.Resolve<ILog>();
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
