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
        private readonly IBroadcastedTransactionRepository _broadcastedTransactionRepository;

        public TransactionMonitorDispatcherRole(
            IBroadcastedTransactionRepository broadcastedTransactionRepository)
        {
            _broadcastedTransactionRepository = broadcastedTransactionRepository;
        }


        public async Task<IEnumerable<Guid>> GetAllOperationIdsAsync()
        {
            return (await _broadcastedTransactionRepository.GetAllAsync())
                .Select(x => x.OperationId)
                .Distinct();
        }
    }
}
