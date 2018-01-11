using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Entities
{
    public class OperationEntity : AzureTableEntity
    {
        public string Amount { get; set; }

        public string FromAddress { get; set; }

        public string GasPrice { get; set; }

        public bool IncludeFee { get; set; }

        public string Nonce { get; set; }

        public Guid OperationId { get; set; }

        public string ToAddress { get; set; }
    }
}
