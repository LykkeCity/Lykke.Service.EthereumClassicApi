using System;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces
{
    public interface ITransactionProcessorDispatcherRole : IActorRole
    {
        Task<string> GetFromAddressAsync(Guid operationId);
    }
}
