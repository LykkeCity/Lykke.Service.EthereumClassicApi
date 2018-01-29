using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IGasPriceRepository
    {
        Task AddOrReplaceAsync(GasPriceDto dto);

        Task<GasPriceDto> TryGetAsync();
    }
}
