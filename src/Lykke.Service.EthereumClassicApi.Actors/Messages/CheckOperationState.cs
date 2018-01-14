﻿using System;
using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class CheckOperationState
    {
        public CheckOperationState(Guid operationId)
        {
            OperationId = operationId;
        }

        public Guid OperationId { get; }
    }
}