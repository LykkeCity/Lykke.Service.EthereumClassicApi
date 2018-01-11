using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces
{
    public interface IHealthMonitorRole
    {
        Task UpdateHealthStatusAsync();
    }
}
