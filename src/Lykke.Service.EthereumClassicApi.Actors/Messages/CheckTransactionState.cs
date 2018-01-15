using System;
using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class CheckTransactionState
    {
        public CheckTransactionState(Guid operationId)
        {
            OperationId = operationId;
        }

        public Guid OperationId { get; }
    }
}
