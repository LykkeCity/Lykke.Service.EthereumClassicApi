using System;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface ITransactionMonitorRole : IActorRole
    {
        Task<bool> CheckTransactionStateAsync(Guid operationId);

        Task DeleteTransactionStateAsync(Guid operationId);
    }
}
