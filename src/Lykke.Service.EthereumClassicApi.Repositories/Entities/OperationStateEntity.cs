using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Entities
{
    public class OperationStateEntity : AzureTableEntity
    {
        public string Amount { get; set; }

        public bool Completed { get; set; }

        public string Error { get; set; }

        public bool Failed { get; set; }

        public string Fee { get; set; }

        public string FromAddress { get; set; }

        public Guid OperationId { get; set; }
        
        public string ToAddress { get; set; }

        public string TxHash { get; set; }

        public DateTimeOffset TxTimestamp { get; set; }
    }
}
