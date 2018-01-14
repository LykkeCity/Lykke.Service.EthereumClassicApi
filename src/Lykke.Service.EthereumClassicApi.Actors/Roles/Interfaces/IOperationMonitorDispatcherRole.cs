using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface IOperationMonitorDispatcherRole : IActorRole
    {
        Task<IEnumerable<Guid>> GetAllOperationIdsAsync();
    }
}
