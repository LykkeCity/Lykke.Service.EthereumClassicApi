using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Services.DTOs;

namespace Lykke.Service.EthereumClassicApi.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<string> BroadcastTransactionAsync(Guid operationId, string signedTxData);

        Task<string> BuildTransactionAsync(BigInteger amount, BigInteger fee, string fromAddress, BigInteger gasPrice, bool includeFee, Guid operationId, string toAddress, BigInteger? gasAmount = null);

        Task<TransactionParamsDto> CalculateTransactionParamsAsync(BigInteger amount, bool includeFee, string toAddress);

        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);
    }
}
