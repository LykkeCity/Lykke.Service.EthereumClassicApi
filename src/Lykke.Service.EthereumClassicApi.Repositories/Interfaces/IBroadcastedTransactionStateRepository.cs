﻿using System;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;

namespace Lykke.Service.EthereumClassicApi.Repositories.Interfaces
{
    public interface IBroadcastedTransactionStateRepository : IBroadcastedTransactionStateQueryRepository
    {
        Task AddOrReplaceAsync(BroadcastedTransactionStateDto dto);

        Task DeleteIfExistAsync(Guid operationId);
    }
}
