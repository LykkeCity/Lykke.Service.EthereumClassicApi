using System;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IBuiltTransactionRepository
    {
        Task AddAsync(BuiltTransactionDto dto);

        Task DeleteIfExistsAsync(Guid operationId);
        
        Task<BuiltTransactionDto> TryGetAsync(Guid operationId);
    }
}
