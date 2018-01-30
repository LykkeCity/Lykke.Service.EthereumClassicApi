using System;
using System.Numerics;
using Lykke.Service.EthereumClassicApi.Common;

namespace Lykke.Service.EthereumClassicApi.Repositories.DTOs
{
    public class TransactionDto
    {
        public BigInteger Amount { get; set; }

        public DateTime? BroadcastedOn { get; set; }

        public DateTime BuiltOn { get; set; }
        
        public DateTime? CompletedOn { get; set; }

        public string Error { get; set; }

        public BigInteger Fee { get; set; }

        public string FromAddress { get; set; }

        public BigInteger GasPrice { get; set; }

        public bool IncludeFee { get; set; }

        public BigInteger Nonce { get; set; }

        public Guid OperationId { get; set; }

        public string SignedTxData { get; set; }

        public string SignedTxHash { get; set; }

        public TransactionState State { get; set; }

        public string ToAddress { get; set; }

        public string TxData { get; set; }
    }
}
