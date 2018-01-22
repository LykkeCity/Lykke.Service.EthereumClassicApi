using System;
using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public class TransactionBroadcasted
    {
        public TransactionBroadcasted(Guid operationId)
        {
            OperationId = operationId;
        }

        public Guid OperationId { get; }
    }
}
