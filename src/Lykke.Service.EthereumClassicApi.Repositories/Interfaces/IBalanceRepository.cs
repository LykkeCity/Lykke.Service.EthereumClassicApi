using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IBalanceRepository
    {
        Task AddOrReplaceAsync(BalanceDto dto);

        Task DeleteAsync(string address);

        Task<IEnumerable<BalanceDto>> GetAsync(int take, string continuationToken);
    }
}
