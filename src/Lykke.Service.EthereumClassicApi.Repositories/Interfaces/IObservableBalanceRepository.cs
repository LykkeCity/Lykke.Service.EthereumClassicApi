using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IObservableBalanceRepository
    {
        Task<bool> DeleteIfExistsAsync(string address);

        Task<bool> ExistsAsync(string address);

        Task<IEnumerable<ObservableBalanceDto>> GetAllAsync();

        Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)> GetAllWithNonZeroAmountAsync(int take, string continuationToken);
        
        Task<bool> TryAddAsync(string address);

        Task<ObservableBalanceDto> TryGetAsync(string address);

        Task UpdateAmountAsync(string address, BigInteger amount);

        Task UpdateLockAsync(string address, bool locked);
    }
}
