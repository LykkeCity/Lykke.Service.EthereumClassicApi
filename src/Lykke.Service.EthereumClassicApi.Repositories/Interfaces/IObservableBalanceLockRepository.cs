using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IObservableBalanceLockRepository
    {
        Task AddOrReplaceAsync(ObservableBalanceLockDto dto);

        Task DeleteIfExistsAsync(string address);

        Task<bool> ExistsAsync(string address);
    }
}
