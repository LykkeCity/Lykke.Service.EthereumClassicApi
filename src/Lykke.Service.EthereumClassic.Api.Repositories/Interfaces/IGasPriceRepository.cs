using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Interfaces
{
    public interface IGasPriceRepository
    {
        Task AddOrReplaceAsync(GasPriceDto dto);

        Task<GasPriceDto> GetAsync();
    }
}