using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Blockchain.Entities;

namespace Lykke.Service.EthereumClassicApi.Blockchain.Interfaces
{
    /// <summary>
    ///     Access layer for requests to Ethereum JavaScript API
    /// </summary>
    public interface IEthereum
    {
        /// <summary>
        ///     Builds unsigned transaction.
        /// </summary>
        /// <param name="to">
        /// </param>
        /// <param name="amount">
        /// </param>
        /// <param name="nonce">
        /// </param>
        /// <param name="gasPrice">
        /// </param>
        /// <param name="gasAmount">
        /// </param>
        /// <returns>
        ///     RLP-encoded transaction data in hex format.
        /// </returns>
        string BuildTransaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice,
            BigInteger gasAmount);

        /// <summary>
        ///     Estimates gas price for simple transfer transaction.
        /// </summary>
        /// <param name="to">
        ///     Destination address.
        /// </param>
        /// <param name="amount">
        ///     Transfer amount.
        /// </param>
        /// <returns>
        ///     A BigInteger instance of the estimated gas price.
        /// </returns>
        Task<BigInteger> EstimateGasPriceAsync(string to, BigInteger amount);

        /// <summary>
        ///     Get the balance of a public address at a specified block.
        /// </summary>
        /// <param name="address">
        ///     The address to get the balance of.
        /// </param>
        /// <param name="blockNumber">
        ///     The number of a block to get the balance at.
        /// </param>
        /// <returns>
        ///     A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        Task<BigInteger> GetBalanceAsync(string address, BigInteger blockNumber);

        Task<string> GetCodeAsync(string address);
            /// <summary>
        ///     Get the balance of a public address at a latest block.
        /// </summary>
        /// <param name="address">
        ///     The address to get the balance of.
        /// </param>
        /// <returns>
        ///     A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        Task<BigInteger> GetLatestBalanceAsync(string address);

        /// <summary>
        ///     Returns the current block number.
        /// </summary>
        /// <returns>
        ///     The number of the most recent block.
        /// </returns>
        Task<BigInteger> GetLatestBlockNumberAsync();

        /// <summary>
        ///     Get next nonce for spwecified address.
        /// </summary>
        /// <param name="address">
        ///     The address to get next nonce of.
        /// </param>
        /// <returns>
        ///     A BigInteger instance of the next nonce for the given address.
        /// </returns>
        Task<BigInteger> GetNextNonceAsync(string address);

        /// <summary>
        ///     Get the balance of a public address at a pending block.
        /// </summary>
        /// <param name="address">
        ///     The address to get the balance of.
        /// </param>
        /// <returns>
        ///     A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        Task<BigInteger> GetPendingBalanceAsync(string address);

        Task<string> GetTransactionErrorAsync(string txHash);


        Task<BigInteger> GetTransactionGasPriceAsync(string txHash);

        /// <summary>
        ///     Get the receipt of a specified transaction.
        /// </summary>
        /// <param name="txHash">
        ///     The transaction hash.
        /// </param>
        /// <returns>
        ///     A transaction receipt object, or null when no receipt was found.
        /// </returns>
        Task<TransactionReceiptEntity> GetTransactionReceiptAsync(string txHash);

        /// <summary>
        ///     Sends an already signed transaction.
        /// </summary>
        /// <param name="signedTxData">
        ///     Signed transaction data in HEX format.
        /// </param>
        /// <returns>
        ///     The 32 Bytes transaction hash as HEX string.
        /// </returns>
        Task<string> SendRawTransactionAsync(string signedTxData);
    }
}
