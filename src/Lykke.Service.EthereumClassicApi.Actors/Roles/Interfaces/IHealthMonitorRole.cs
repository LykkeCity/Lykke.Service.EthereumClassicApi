using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface IHealthMonitorRole
    {
        Task UpdateHealthStatusAsync();
    }
}
