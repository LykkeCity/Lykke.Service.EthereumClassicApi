using Autofac;
using Lykke.Service.EthereumClassicApi.Blockchain.Factories;
using Lykke.Service.EthereumClassicApi.Blockchain.Factories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Blockchain
{
    public class BlockchainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<EthereumFactory>()
                .As<IEthereumFactory>()
                .SingleInstance();

            builder
                .Register(c => c.Resolve<IEthereumFactory>().Build())
                .SingleInstance();
        }
    }
}