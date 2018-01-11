using System.Net.Http;
using Autofac;
using Lykke.Service.EthereumClassic.Api.Blockchain;
using Lykke.Service.EthereumClassic.Api.Repositories;
using Lykke.Service.EthereumClassic.Api.Services.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Services
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
