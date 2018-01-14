using System.Net.Http;
using Autofac;
using Lykke.Service.EthereumClassicApi.Blockchain;
using Lykke.Service.EthereumClassicApi.Repositories;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Services
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterModule<BlockchainModule>()
                .RegisterModule<RepositoriesModule>();
            
            builder
                .RegisterType<GasPriceOracleService>()
                .As<IGasPriceOracleService>()
                .SingleInstance();
            
            builder
                .RegisterType<TransactionStateService>()
                .As<ITransactionStateService>()
                .SingleInstance();

            builder
                .RegisterType<HttpClient>()
                .AsSelf();
        }
    }
}
