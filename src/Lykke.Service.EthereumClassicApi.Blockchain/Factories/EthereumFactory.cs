using System;
using Lykke.Service.EthereumClassicApi.Blockchain.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Nethereum.Parity;

namespace Lykke.Service.EthereumClassicApi.Blockchain.Factories
{
    public class EthereumFactory : IEthereumFactory
    {
        private readonly EthereumClassicApiSettings _serviceSettings;

        public EthereumFactory(
            EthereumClassicApiSettings serviceSettings)
        {
            _serviceSettings = serviceSettings;
        }

        private IEthereum BuildGeth()
        {
            throw new NotSupportedException("Geth is not supported yet");
        }

        private IEthereum BuildParity()
        {
            var web3Parity = new Web3Parity(_serviceSettings.EthereumRpcNodeUrl);

            return new Parity(web3Parity);
        }

        public IEthereum Build()
        {
            var nodeType = _serviceSettings.EthereumRpcNodeType;

            switch (nodeType.ToLowerInvariant())
            {
                case "geth":
                    return BuildGeth();
                case "parity":
                    return BuildParity();
                default:
                    throw new NotSupportedException($"{nodeType} is not supported Ethereum client.");
            }
        }
    }
}
