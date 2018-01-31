using Lykke.Service.EthereumClassicApi.Services.DTOs;

namespace Lykke.Service.EthereumClassicApi.Services.Interfaces
{
    public interface IHealthService
    {
        HealthStatusDto GetHealthStatus();
    }
}
