using System;
using System.Numerics;

namespace Lykke.Service.EthereumClassicApi.Repositories.DTOs
{
    public class BroadcastedTransactionDto
    {
        public BigInteger Amount { get; set; }

        public BigInteger Fee { get; set; }

        public string FromAddress { get; set; }
        
        public Guid OperationId { get; set; }

        public string SignedTxData { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string ToAddress { get; set; }

        public string TxHash { get; set; }
    }
}
