using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.Parity;
using Newtonsoft.Json.Linq;

namespace Lykke.Service.EthereumClassicApi.Blockchain
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
            var request = new RpcRequest($"{NewGuid.Get()}", "parity_nextNonce", address);
            var response = await _web3Parity.Client.SendRequestAsync<string>(request);
            var result = new HexBigInteger(response);

            return result.Value;
        }

        public override async Task<string> GetTransactionErrorAsync(string txHash)
        {
            var request = new RpcRequest($"{NewGuid.Get()}", "trace_transaction", txHash);
            var response = await _web3Parity.Client.SendRequestAsync<JArray>(request);

            return response?.Select(x => x?["error"]?.ToString()).FirstOrDefault();
        }
    }
}
