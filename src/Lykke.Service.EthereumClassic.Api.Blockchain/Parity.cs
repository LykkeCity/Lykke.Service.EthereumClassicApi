using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Common.Utils;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.Parity;

namespace Lykke.Service.EthereumClassic.Api.Blockchain
{
    public class Parity : EthereumBase
    {
        private readonly Web3Parity _web3Parity;

        public Parity(Web3Parity web3Parity) 
            : base(web3Parity)
        {
            _web3Parity = web3Parity;
        }

        public override async Task<BigInteger> GetNextNonceAsync(string address)
        {
            var request  = new RpcRequest($"{NewGuid.Get():N}", "parity_nextNonce", address);
            var response = await _web3Parity.Client.SendRequestAsync<string>(request);
            var result   = new HexBigInteger(response);

            return result.Value;
        }
    }
}