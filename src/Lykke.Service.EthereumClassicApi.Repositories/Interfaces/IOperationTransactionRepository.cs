using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IOperationTransactionRepository
    {
        Task AddAsync(OperationTransactionDto dto);

        Task DeleteAsync(Guid operationId);

        Task<bool> ExistsAsync(Guid operation, string signedTxData);

        Task<IEnumerable<OperationTransactionDto>> GetAsync(Guid operationId);
    }
}
