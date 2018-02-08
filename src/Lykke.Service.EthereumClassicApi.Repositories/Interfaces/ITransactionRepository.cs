using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(BuiltTransactionDto dto);

        Task<bool> DeleteIfExistsAsync(Guid operationId);

        Task<IEnumerable<TransactionEntity>> GetAllAsync(Guid operationId);

        Task<IEnumerable<TransactionEntity>> GetAllInProgressAsync();

        Task UpdateAsync(BroadcastedTransactionDto dto);

        Task UpdateAsync(CompletedTransactionDto dto);
    }
}
