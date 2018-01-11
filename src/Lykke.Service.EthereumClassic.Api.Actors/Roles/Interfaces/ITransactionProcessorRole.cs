using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces
{
    public interface ITransactionProcessorRole : IActorRole
    {
        Task<string> BroadcastTransaction(Guid operationId, string signedTxData);

        Task<string> BuildOperationAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress);

        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);
    }
}
