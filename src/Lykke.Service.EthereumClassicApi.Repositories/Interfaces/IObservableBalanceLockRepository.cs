using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IObservableBalanceLockRepository
    {
        Task AddAsync(ObservableBalanceLockDto dto);

        Task DeleteAsync(string address);

        Task<bool> ExistsAsync(string address);
    }
}
