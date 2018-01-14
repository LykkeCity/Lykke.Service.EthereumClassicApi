using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IBalanceQueryRepository
    {
        Task<(IEnumerable<BalanceDto> Balances, string ContinuationToken)> GetAsync(int take, string continuationToken);
    }
}
