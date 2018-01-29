using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Entities
{
    public class BroadcastedTransactionEntity : AzureTableEntity
    {
        public string Amount { get; set; }

        public DateTime CreatedOn { get; set; }

        public string FromAddress { get; set; }

        public Guid OperationId { get; set; }

        public string SignedTxData { get; set; }

        public string ToAddress { get; set; }

        public string TxHash { get; set; }
    }
}
