using System;
using Lykke.Service.EthereumClassic.Api.Blockchain.Factories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Blockchain.Interfaces;
using Lykke.Service.EthereumClassic.Api.Common.Settings;
using Nethereum.Parity;

namespace Lykke.Service.EthereumClassic.Api.Blockchain.Factories
{
    public class EthereumFactory : IEthereumFactory
    {
        private readonly EthereumClassicApiSettings _serviceSettings;

        public EthereumFactory(
            EthereumClassicApiSettings serviceSettings)
        {
            _serviceSettings = serviceSettings;
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

        private IEthereum BuildGeth()
        {
            throw new NotImplementedException("Geth is not supported yet");
        }

        private IEthereum BuildParity()
        {
            var web3Parity = new Web3Parity(_serviceSettings.EthereumRpcNodeUrl);

            return new Parity(web3Parity);
        }
    }
}