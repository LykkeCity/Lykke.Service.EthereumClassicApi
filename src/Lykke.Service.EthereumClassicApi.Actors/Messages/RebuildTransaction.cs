using System;
using System.ComponentModel;


namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class RebuildTransaction
    {
        public RebuildTransaction(decimal feeFactor, Guid operationId)
        {
            FeeFactor   = feeFactor;
            OperationId = operationId;
        }

        
        public decimal FeeFactor { get; }
        
        public Guid OperationId { get; }
    }
}
