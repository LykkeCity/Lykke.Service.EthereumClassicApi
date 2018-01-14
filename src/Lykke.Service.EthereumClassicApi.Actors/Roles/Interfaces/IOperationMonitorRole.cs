using System;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface IOperationMonitorRole : IActorRole
    {
        Task<bool> CheckOperationAsync(Guid operationId);
    }
}
