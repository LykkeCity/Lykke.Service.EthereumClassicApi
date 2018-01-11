using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces
{
    public interface IBalanceObserverManagerRole : IActorRole
    {
        Task BeginBalanceMonitoringAsync(string address);

        Task EndBalanceMonitoringAsync(string address);
    }
}
