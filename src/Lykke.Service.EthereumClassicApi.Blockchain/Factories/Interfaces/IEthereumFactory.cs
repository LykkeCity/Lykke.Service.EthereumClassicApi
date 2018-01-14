using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Blockchain.Factories.Interfaces
{
    public interface IEthereumFactory
    {
        IEthereum Build();
    }
}