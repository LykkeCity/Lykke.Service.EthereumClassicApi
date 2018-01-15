using System;
using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class DeleteTransactionState
    {
        public DeleteTransactionState(Guid operationId)
        {
            OperationId = operationId;
        }

        public Guid OperationId { get; }
    }
}
