using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IBalanceRepository : IBalanceQueryRepository
    {
        Task AddOrReplaceAsync(BalanceDto dto);

        Task DeleteAsync(string address);
    }
}
