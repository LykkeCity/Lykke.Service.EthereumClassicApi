using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Services.Interfaces
{
    public interface ITransactionBuilderService
    {
        Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress);

        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);
    }
}
