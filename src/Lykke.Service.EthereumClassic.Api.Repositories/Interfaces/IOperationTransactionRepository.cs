using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Interfaces
{
    public interface IOperationTransactionRepository
    {
        Task AddAsync(OperationTransactionDto dto);

        Task DeleteAsync(Guid operationId);

        Task<bool> ExistsAsync(Guid operation, string signedTxData);

        Task<IEnumerable<OperationTransactionDto>> GetAsync(Guid operationId);
    }
}
