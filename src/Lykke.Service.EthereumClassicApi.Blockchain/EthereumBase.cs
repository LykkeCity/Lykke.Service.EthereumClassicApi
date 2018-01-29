using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Blockchain.Entities;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;
using Transaction = Nethereum.Signer.Transaction;

namespace Lykke.Service.EthereumClassicApi.Blockchain
{
    public abstract class EthereumBase : IEthereum
    {
        private readonly Web3 _web3;


        protected EthereumBase(
            Web3 web3)
        {
            _web3 = web3;
        }

        private async Task<BigInteger> GetBalanceAsync(string address, BlockParameter blockParameter)
        {
            return (await _web3.Eth.GetBalance.SendRequestAsync(address, blockParameter))
                .Value;
        }

        public string BuildTransaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice,
            BigInteger gasAmount)
        {
            var transaction = new Transaction
            (
                to,
                amount,
                nonce,
                gasPrice,
                gasAmount
            );

            return transaction.GetRLPEncoded().ToHex();
        }

        public async Task<BigInteger> EstimateGasPriceAsync(string to, BigInteger amount)
        {
            var input = new TransactionInput
            {
                To = to,
                Value = new HexBigInteger(amount)
            };

            return (await _web3.Eth.Transactions.EstimateGas.SendRequestAsync(input))
                .Value;
        }

        public async Task<BigInteger> GetBalanceAsync(string address, BigInteger blockNumber)
        {
            var block = new BlockParameter((ulong) blockNumber);

            return await GetBalanceAsync(address, block);
        }

        public async Task<string> GetCodeAsync(string address)
        {
            return await _web3.Eth.GetCode.SendRequestAsync(address);
        }

        public async Task<BigInteger> GetLatestBalanceAsync(string address)
        {
            var block = BlockParameter.CreateLatest();

            return await GetBalanceAsync(address, block);
        }

        public async Task<BigInteger> GetLatestBlockNumberAsync()
        {
            return await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
        }

        public abstract Task<BigInteger> GetNextNonceAsync(string address);

        public async Task<BigInteger> GetTransactionGasPriceAsync(string txHash)
        {
            return (await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txHash))
                .GasPrice.Value;
        }

        public async Task<BigInteger> GetPendingBalanceAsync(string address)
        {
            var block = BlockParameter.CreatePending();

            return await GetBalanceAsync(address, block);
        }

        public async Task<TransactionReceiptEntity> GetTransactionReceiptAsync(string txHash)
        {
            // _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash) fails with exception
            // using workaround

            var request = _web3.Eth.Transactions.GetTransactionReceipt.BuildRequest(txHash);
            var receipt = await _web3.Client.SendRequestAsync<JObject>(request);

            if (receipt != null)
            {
                return new TransactionReceiptEntity
                {
                    BlockHash = receipt["blockHash"].Value<string>(),
                    BlockNumber = new HexBigInteger(receipt["blockNumber"].Value<string>()).Value,
                    ContractAddress = receipt["contractAddress"].Value<string>(),
                    CumulativeGasUsed = new HexBigInteger(receipt["cumulativeGasUsed"].Value<string>()).Value,
                    GasUsed = new HexBigInteger(receipt["gasUsed"].Value<string>()).Value,
                    Status = new HexBigInteger(receipt["status"].Value<string>()).Value,
                    TransactionHash = receipt["transactionHash"].Value<string>(),
                    TransactionIndex = new HexBigInteger(receipt["transactionIndex"].Value<string>()).Value
                };
            }

            return null;
        }

        public abstract Task<string> GetTransactionErrorAsync(string txHash);

        public async Task<string> SendRawTransactionAsync(string signedTxData)
        {
            return await _web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTxData);
        }
    }
}
