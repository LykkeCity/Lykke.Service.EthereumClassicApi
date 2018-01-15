using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface ITransactionProcessorRole : IActorRole
    {
        /// <summary>
        ///    Broadcasts specified signed transaction.
        /// </summary>
        /// <param name="operationId">
        ///    The identifier of operation that relates to the signed transaction.
        /// </param>
        /// <param name="signedTxData">
        ///    Signed transaction data.
        /// </param>
        /// <exception cref="Exceptions.ConflictException">
        ///    Thrown when specified signed transaction fro specified operation id has already bbeen bbroadcasted.
        /// </exception>
        Task<string> BroadcastTransaction(Guid operationId, string signedTxData);

        Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress);

        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);
    }
}
