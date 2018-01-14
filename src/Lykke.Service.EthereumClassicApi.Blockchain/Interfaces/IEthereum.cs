using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Blockchain.Entities;

namespace Lykke.Service.EthereumClassicApi.Blockchain.Interfaces
{
    /// <summary>
    ///    Access layer for requests to Ethereum JavaScript API
    /// </summary>
    public interface IEthereum
    {
        // TODO: Add documentation
        string BuildTransaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice, BigInteger gasAmount);

        // TODO: Add documentation
        Task<BigInteger> EstimateGasPriceAsync(string to, BigInteger amount);

        /// <summary>
        ///    Get the balance of a public address at a specified block.
        /// </summary>
        /// <param name="address">
        ///    The address to get the balance of.
        /// </param>
        /// <param name="blockNumber">
        ///    The number of a block to get the balance at.
        /// </param>
        /// <returns>
        ///    A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        Task<BigInteger> GetBalanceAsync(string address, BigInteger blockNumber);

        /// <summary>
        ///    Get the balance of a public address at a latest block.
        /// </summary>
        /// <param name="address">
        ///    The address to get the balance of.
        /// </param>
        /// <returns>
        ///    A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        Task<BigInteger> GetLatestBalanceAsync(string address);

        /// <summary>
        ///    Returns the current block number.
        /// </summary>
        /// <returns>
        ///    The number of the most recent block.
        /// </returns>
        Task<BigInteger> GetLatestBlockNumberAsync();

        /// <summary>
        ///    Get the balance of a public address at a pending block.
        /// </summary>
        /// <param name="address">
        ///    The address to get the balance of.
        /// </param>
        /// <returns>
        ///    A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        Task<BigInteger> GetPendingBalanceAsync(string address);

        // TODO: Add documentation
        Task<BigInteger> GetNextNonceAsync(string address);

        /// <summary>
        ///    Get the receipt of a specified transaction.
        /// </summary>
        /// <param name="txHash">
        ///     The transaction hash.
        /// </param>
        /// <returns>
        ///    A transaction receipt object, or null when no receipt was found.
        /// </returns>
        Task<TransactionReceiptEntity> GetTransactionReceipt(string txHash);

        /// <summary>
        ///    Sends an already signed transaction.
        /// </summary>
        /// <param name="signedTxData">
        ///    Signed transaction data in HEX format.
        /// </param>
        /// <returns>
        ///    The 32 Bytes transaction hash as HEX string.
        /// </returns>
        Task<string> SendRawTransactionAsync(string signedTxData);
    }
}
