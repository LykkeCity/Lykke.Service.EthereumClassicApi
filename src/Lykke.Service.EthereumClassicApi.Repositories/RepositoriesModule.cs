using Autofac;
using Lykke.Service.EthereumClassicApi.Repositories.Factories;
using Lykke.Service.EthereumClassicApi.Repositories.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories
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
                .Register(c => c.Resolve<IRepositoryFactory>().BuildGasPriceRepository())
                .AsSelf()
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildObservableBalanceRepository())
                .AsSelf()
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildObservableBalanceLockRepository())
                .AsSelf()
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildBuiltTransactionRepository())
                .AsSelf()
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildBroadcastedTransactionStateRepository())
                .AsSelf()
                .As<IBroadcastedTransactionStateQueryRepository>()
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IRepositoryFactory>().BuildBroadcastedTransactionRepository())
                .AsSelf()
                .SingleInstance();

            builder
                .Register(c => new HealthStatusRepository())
                .As<IHealthStatusRepository>()
                .SingleInstance();
        }
    }
}
