using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(BuiltTransactionDto dto);

        Task<bool> DeleteIfExistsAsync(Guid operationId);

        Task<IEnumerable<TransactionDto>> GetAllAsync(Guid operationId);

        Task<IEnumerable<TransactionDto>> GetAllInProgressAsync();

        Task UpdateAsync(BroadcastedTransactionDto dto);

        Task UpdateAsync(CompletedTransactionDto dto);
    }
}
