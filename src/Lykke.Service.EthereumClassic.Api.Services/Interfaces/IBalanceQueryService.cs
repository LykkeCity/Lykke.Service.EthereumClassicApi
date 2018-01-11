using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Services.DTOs;

namespace Lykke.Service.EthereumClassic.Api.Services.Interfaces
{
    public interface IBalanceQueryService
    {
        Task<(IEnumerable<BalanceDto>, string)> GetBalancesAsync(int take, string continuationToken);
    }
}
