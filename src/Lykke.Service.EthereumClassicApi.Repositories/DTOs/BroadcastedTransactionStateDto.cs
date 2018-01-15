using System;
using System.Numerics;
using Lykke.Service.EthereumClassicApi.Common;

namespace Lykke.Service.EthereumClassicApi.Repositories.DTOs
{
    public class BroadcastedTransactionStateDto
    {
        public BigInteger Amount { get; set; }
        
        public string Error { get; set; }
        
        public BigInteger Fee { get; set; }

        public string FromAddress { get; set; }

        public Guid OperationId { get; set; }

        public TransactionState State { get; set; }

        public DateTimeOffset Timestamp { get; set; }
        
        public string ToAddress { get; set; }

        public string TxHash { get; set; }
    }
}
