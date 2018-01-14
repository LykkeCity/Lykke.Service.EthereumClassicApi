using System;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class TransactionProcessorDispatcherRole : ITransactionProcessorDispatcherRole
    {
        private readonly IOperationRepository _operationRepository;


        public TransactionProcessorDispatcherRole(
            IOperationRepository operationRepository)
        {
            _operationRepository = operationRepository;
        }


        public async Task<string> GetFromAddressAsync(Guid operationId)
        {
            return (await _operationRepository.GetAsync(operationId)).FromAddress;
        }
    }
}
