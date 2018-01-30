using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<string> BroadcastTransactionAsync(Guid operationId, string signedTxData);

        Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress);

        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);
    }
}
