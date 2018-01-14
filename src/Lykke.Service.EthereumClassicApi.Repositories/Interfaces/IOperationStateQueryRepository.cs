using System;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IOperationStateQueryRepository
    {
        Task<OperationStateDto> GetAsync(Guid operationId);
    }
}
