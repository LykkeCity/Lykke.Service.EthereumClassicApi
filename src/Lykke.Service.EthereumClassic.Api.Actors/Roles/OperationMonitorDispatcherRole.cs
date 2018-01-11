using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Actors.Roles
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
