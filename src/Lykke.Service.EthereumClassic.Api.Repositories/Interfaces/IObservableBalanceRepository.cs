using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Interfaces
{
    public interface IObservableBalanceRepository
    {

        Task AddAsync(ObservableBalanceDto dto);

        Task DeleteAsync(string address);

        Task<bool> ExistsAsync(string address);

        Task<IEnumerable<ObservableBalanceDto>> GetAllAsync();
    }
}
