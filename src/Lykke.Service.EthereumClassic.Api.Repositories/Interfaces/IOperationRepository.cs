using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Interfaces
{
    public interface IOperationRepository
    {
        Task AddAsync(OperationDto dto);

        Task DeleteAsync(Guid operationId);

        Task<OperationDto> GetAsync(Guid operationId);

        Task<IEnumerable<Guid>> GetAllOperationIdsAsync();
    }
}
