using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class OperationMonitorDispatcherRole : IOperationMonitorDispatcherRole
    {
        private readonly IOperationRepository _operationRepository;


        public OperationMonitorDispatcherRole(
            IOperationRepository operationRepository)
        {
            _operationRepository = operationRepository;
        }


        public async Task<IEnumerable<Guid>> GetAllOperationIdsAsync()
        {
            return await _operationRepository.GetAllOperationIdsAsync();
        }
    }
}
