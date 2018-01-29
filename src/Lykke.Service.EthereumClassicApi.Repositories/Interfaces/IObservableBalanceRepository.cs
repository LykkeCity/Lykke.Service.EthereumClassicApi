using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IObservableBalanceRepository : IObservableBalanceQueryRepository
    {
        Task AddAsync(ObservableBalanceDto dto);

        Task DeleteAsync(string address);

        Task<bool> ExistsAsync(string address);

        Task<IEnumerable<ObservableBalanceDto>> GetAllAsync();

        Task ReplaceAsync(ObservableBalanceDto dto);
    }
}
