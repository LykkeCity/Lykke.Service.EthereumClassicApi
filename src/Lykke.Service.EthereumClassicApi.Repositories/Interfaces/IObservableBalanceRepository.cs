using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IObservableBalanceRepository
    {
        Task AddAsync(ObservableBalanceDto dto);

        Task DeleteAsync(string address);

        Task<bool> ExistsAsync(string address);

        Task<ObservableBalanceDto> TryGetAsync(string address);

        Task<IEnumerable<ObservableBalanceDto>> GetAllAsync();

        Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)> GetAllWithNonZeroAmountAsync(int take, string continuationToken);

        Task UpdateAsync(string address, BigInteger? amount = null, bool? locked = null);
    }
}
