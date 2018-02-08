using System;
using Lykke.Service.EthereumClassicApi.Common;

namespace Lykke.Service.EthereumClassicApi.Repositories.DTOs
{
    public class BroadcastedTransactionDto
    {
        public DateTime BroacastedOn { get; set; }

        public Guid OperationId { get; set; }

        public string SignedTxData { get; set; }

        public string SignedTxHash { get; set; }

        public TransactionState State { get; set; }

        public string TxData { get; set; }
    }
}
