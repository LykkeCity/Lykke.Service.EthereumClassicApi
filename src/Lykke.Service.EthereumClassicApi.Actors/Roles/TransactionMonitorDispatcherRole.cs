using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class TransactionMonitorDispatcherRole : ITransactionMonitorDispatcherRole
    {
        private readonly ITransactionRepository _transactionRepository;


        public TransactionMonitorDispatcherRole(
            ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }


        public async Task<IEnumerable<Guid>> GetAllInProgressOperationIdsAsync()
        {
            return (await _transactionRepository.GetAllInProgressAsync())
                .Select(x => x.OperationId)
                .Distinct();
        }
    }
}
