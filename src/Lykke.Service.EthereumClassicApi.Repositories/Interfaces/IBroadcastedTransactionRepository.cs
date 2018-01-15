using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IBroadcastedTransactionRepository
    {
        Task AddAsync(BroadcastedTransactionDto dto);

        Task DeleteAsync(Guid operationId);

        Task<bool> ExistsAsync(Guid operation, string signedTxData);

        Task<IEnumerable<BroadcastedTransactionDto>> GetAsync(Guid operationId);
    }
}
