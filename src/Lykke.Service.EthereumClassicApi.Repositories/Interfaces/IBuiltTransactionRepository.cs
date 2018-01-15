using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IBuiltTransactionRepository
    {
        Task AddAsync(BuiltTransactionDto dto);

        Task DeleteAsync(Guid operationId);

        Task<BuiltTransactionDto> GetAsync(Guid operationId);

        Task<IEnumerable<Guid>> GetAllOperationIdsAsync();
    }
}
