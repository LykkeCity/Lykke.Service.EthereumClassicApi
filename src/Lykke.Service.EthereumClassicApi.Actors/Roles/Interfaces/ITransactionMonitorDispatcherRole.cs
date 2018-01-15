using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface ITransactionMonitorDispatcherRole : IActorRole
    {
        Task<IEnumerable<Guid>> GetAllOperationIdsAsync();
    }
}
