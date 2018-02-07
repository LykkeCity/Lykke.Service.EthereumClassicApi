using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;


namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IObservableBalanceRepository
    {
        Task<bool> DeleteIfExistsAsync(string address);

        Task<bool> ExistsAsync(string address);

        Task<(IEnumerable<ObservableBalanceEntity> Balances, string ContinuationToken)> GetAllAsync(int take, string continuationToken);

        Task<(IEnumerable<ObservableBalanceEntity> Balances, string ContinuationToken)> GetAllWithNonZeroAmountAsync(int take, string continuationToken);
        
        Task<bool> TryAddAsync(string address);

        Task<ObservableBalanceEntity> TryGetAsync(string address);

        Task UpdateAmountAsync(string address, BigInteger amount);

        Task UpdateLockAsync(string address, bool locked);
    }
}
