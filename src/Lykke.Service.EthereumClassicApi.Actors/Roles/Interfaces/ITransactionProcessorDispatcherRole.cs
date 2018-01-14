using System;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface ITransactionProcessorDispatcherRole : IActorRole
    {
        Task<string> GetFromAddressAsync(Guid operationId);
    }
}
