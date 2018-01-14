using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IHealthStatusRepository
    {
        HealthStatusDto Get();

        void Update(HealthStatusDto dto);
    }
}
