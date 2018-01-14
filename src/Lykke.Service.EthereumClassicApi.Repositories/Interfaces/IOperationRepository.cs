using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IOperationRepository
    {
        Task AddAsync(OperationDto dto);

        Task DeleteAsync(Guid operationId);

        Task<OperationDto> GetAsync(Guid operationId);

        Task<IEnumerable<Guid>> GetAllOperationIdsAsync();
    }
}
