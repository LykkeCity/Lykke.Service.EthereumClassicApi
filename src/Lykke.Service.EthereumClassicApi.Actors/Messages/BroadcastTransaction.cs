using System;
using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class BroadcastTransaction
    {
        public BroadcastTransaction(
            Guid operationId,
            string signedTxData)
        {
            OperationId = operationId;
            SignedTxData = signedTxData;
        }


        public Guid OperationId { get; }

        public string SignedTxData { get; }
    }
}
