using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Interfaces
{
    public interface IHealthStatusRepository
    {
        HealthStatusDto Get();

        void Update(HealthStatusDto dto);
    }
}
