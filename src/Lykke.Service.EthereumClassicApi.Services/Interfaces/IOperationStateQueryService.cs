using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Services.DTOs;

namespace Lykke.Service.EthereumClassicApi.Services.Interfaces
{
    public interface IOperationStateQueryService
    {
        Task<IEnumerable<OperationStateDto>> GetCompletedOperationsAsync(int take, string continuationToken);

        Task<IEnumerable<OperationStateDto>> GetFailedOperationsAsync(int take, string continuationToken);

        Task<IEnumerable<OperationStateDto>> GetInProgressOperationsAsync(int take, string continuationToken);
    }
}
