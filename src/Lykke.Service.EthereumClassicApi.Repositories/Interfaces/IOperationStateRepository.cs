﻿using System;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;


namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IOperationStateRepository : IOperationStateQueryRepository
    {
        Task AddOrReplaceAsync(OperationStateDto dto);

        Task DeleteAsync(Guid operationId);
    }
}
