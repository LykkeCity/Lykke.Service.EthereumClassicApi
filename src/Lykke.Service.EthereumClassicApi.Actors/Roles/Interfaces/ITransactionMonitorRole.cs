﻿using System;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface ITransactionMonitorRole : IActorRole
    {
        Task<bool> CheckTransactionStatesAsync(Guid operationId);
    }
}
