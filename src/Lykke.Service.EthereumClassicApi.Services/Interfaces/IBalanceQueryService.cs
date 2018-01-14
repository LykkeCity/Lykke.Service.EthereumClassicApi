using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Services.DTOs;

namespace Lykke.Service.EthereumClassicApi.Services.Interfaces
{
    public interface IBalanceQueryService
    {
        Task<(IEnumerable<BalanceDto>, string)> GetBalancesAsync(int take, string continuationToken);
    }
}
