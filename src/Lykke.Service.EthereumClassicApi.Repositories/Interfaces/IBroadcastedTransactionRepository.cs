using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IBroadcastedTransactionRepository
    {
        Task AddOrReplaceAsync(BroadcastedTransactionDto dto);

        Task DeleteAsync(Guid operationId);

        Task<IEnumerable<BroadcastedTransactionDto>> GetAllAsync();

        Task<bool> ExistsAsync(Guid operationId, string signedTxData);

        Task<IEnumerable<BroadcastedTransactionDto>> GetAsync(Guid operationId);
    }
}
