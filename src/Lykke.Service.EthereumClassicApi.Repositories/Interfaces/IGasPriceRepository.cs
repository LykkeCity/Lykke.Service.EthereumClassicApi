using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IGasPriceRepository
    {
        Task AddOrReplaceAsync(GasPriceEntity dto);

        Task<GasPriceEntity> TryGetAsync();
    }
}
