using System;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IBroadcastedTransactionStateRepository
    {
        Task AddOrReplaceAsync(BroadcastedTransactionStateDto dto);

        Task DeleteIfExistAsync(Guid operationId);

        Task<bool> ExistsAsync(Guid operationId);

        Task<BroadcastedTransactionStateDto> TryGetAsync(Guid operationId);
    }
}
