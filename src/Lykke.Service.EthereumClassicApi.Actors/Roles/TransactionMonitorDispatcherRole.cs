using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class TransactionMonitorDispatcherRole : ITransactionMonitorDispatcherRole
    {
        private readonly IBuiltTransactionRepository _builtTransactionRepository;


        public TransactionMonitorDispatcherRole(
            IBuiltTransactionRepository builtTransactionRepository)
        {
            _builtTransactionRepository = builtTransactionRepository;
        }


        public async Task<IEnumerable<Guid>> GetAllOperationIdsAsync()
        {
            return await _builtTransactionRepository.GetAllOperationIdsAsync();
        }
    }
}
