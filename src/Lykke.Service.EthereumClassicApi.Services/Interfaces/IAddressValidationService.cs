using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Services.Interfaces
{
    public interface IAddressValidationService
    {
        Task<bool> ValidateAddressAsync(string address);
    }
}
