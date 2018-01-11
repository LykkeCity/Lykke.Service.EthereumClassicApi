using System;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces
{
    public interface IOperationMonitorRole : IActorRole
    {
        Task<bool> CheckOperationAsync(Guid operationId);
    }
}
