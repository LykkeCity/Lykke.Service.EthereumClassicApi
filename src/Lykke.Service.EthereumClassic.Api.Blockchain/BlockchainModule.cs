using Autofac;
using Lykke.Service.EthereumClassic.Api.Blockchain.Factories;
using Lykke.Service.EthereumClassic.Api.Blockchain.Factories.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Blockchain
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