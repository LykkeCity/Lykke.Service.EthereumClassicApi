using Autofac;
using Lykke.Service.EthereumClassicApi.Repositories.Factories;
using Lykke.Service.EthereumClassicApi.Repositories.Factories.Interfaces;

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
                .Register(c => c.Resolve<IRepositoryFactory>().BuildTransactionRepository())
                .AsSelf()
                .SingleInstance();
        }
    }
}
