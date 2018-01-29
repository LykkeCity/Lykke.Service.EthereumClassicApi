using System;
using System.ComponentModel;
using System.Numerics;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class BuildTransaction
    {
        public BuildTransaction(BigInteger amount, string fromAddress, bool includeFee, Guid operationId,
            string toAddress)
        {
            Amount = amount;
            FromAddress = fromAddress;
            IncludeFee = includeFee;
            OperationId = operationId;
            ToAddress = toAddress;
        }

        public BigInteger Amount { get; }

        public string FromAddress { get; }

        public bool IncludeFee { get; }

        public Guid OperationId { get; }

        public string ToAddress { get; }
    }
}
