using System;
using Akka.Actor;
using Akka.Configuration;
using Autofac;
using Lykke.Service.EthereumClassic.Api.Actors.Extensions;
using Lykke.Service.EthereumClassic.Api.Actors.Factories;
using Lykke.Service.EthereumClassic.Api.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Actors.Messages;
using Lykke.Service.EthereumClassic.Api.Common.Settings;
using Lykke.Service.EthereumClassic.Api.Logger;
using Lykke.SettingsReader;


namespace Lykke.Service.EthereumClassic.Api.Actors
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

        public static IActorSystemFacade Build(IReloadingManager<AppSettings> settings)
        {
            var container = BuildContainer(settings);
            
            LykkeLogger.Configure(container);
            
            var actorSystem   = BuildActorSystem(container);
            var actorFactory  = new RootActorFactory(actorSystem);
            var facadeFactory = new ActorSystemFacadeFactory(actorFactory, settings.CurrentValue.EthereumClassicApi, actorSystem.Scheduler);
            

            return facadeFactory.Build();
        }

        private static ActorSystem BuildActorSystem(IContainer container)
        {
            var systemConfig = ConfigurationFactory.FromResource
            (
                "Lykke.Service.EthereumClassic.Api.Actors.SystemConfig.json",
                typeof(ActorSystemFacadeFactory).Assembly
            );

            return ActorSystem
                .Create("ethereum-classic", systemConfig)
                .WithContainer(container);
        }

        private static IContainer BuildContainer(IReloadingManager<AppSettings> settings)
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder
                .RegisterModule(new ActorsModule(settings));

            return containerBuilder.Build();
        }
    }
}
