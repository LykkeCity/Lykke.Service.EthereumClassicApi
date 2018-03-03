using System;
using System.Numerics;
using Lykke.Service.EthereumClassicApi.Common;

namespace Lykke.Service.EthereumClassicApi.Repositories.DTOs
{
    public class CompletedTransactionDto
    {
        public BigInteger BlockNumber { get; set; }

        public DateTime CompletedOn { get; set; }

        public string Error { get; set; }

        public Guid OperationId { get; set; }

        public TransactionState State { get; set; }

        public string TxData { get; set; }
    }
}
