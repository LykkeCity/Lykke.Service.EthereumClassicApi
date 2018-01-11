using System;
using Autofac;
using Lykke.Service.EthereumClassic.Api.Repositories.Factories;
using Lykke.Service.EthereumClassic.Api.Repositories.Factories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Repositories
{
    public class RepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<RepositoryFactory>()
                .As<IRepositoryFactory>()
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildBalanceRepository())
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildGasPriceRepository())
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildObservableBalanceRepository())
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildOperationRepository())
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildOperationStateRepository())
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildOperationTransactionRepository())
                .SingleInstance();

            builder
                .Register(c => new HealthStatusRepository())
                .As<IHealthStatusRepository>()
                .SingleInstance();
        }
    }
}
