using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IObservableBalanceQueryRepository
    {
        Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)> GetAllWithNonZeroAmountAsync(int take, string continuationToken);
    }
}
