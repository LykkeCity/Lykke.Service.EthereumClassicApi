using Lykke.Service.EthereumClassic.Api.Blockchain.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Blockchain.Factories.Interfaces
{
    public interface IEthereumFactory
    {
        IEthereum Build();
    }
}