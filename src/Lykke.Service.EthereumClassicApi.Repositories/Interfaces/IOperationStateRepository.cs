using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IOperationStateRepository
    {
        Task AddOrReplaceAsync(OperationStateDto dto);

        Task DeleteAsync(Guid operationId);

        Task DeleteInProgressAsync(Guid operationId);

        Task<IEnumerable<OperationStateDto>> GetAsync(bool completed, bool failed, int take, string continuationToken);
    }
}
