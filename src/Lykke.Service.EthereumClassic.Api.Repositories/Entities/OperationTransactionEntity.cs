﻿using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Entities
{
    public class OperationTransactionEntity : AzureTableEntity
    {
        public string Amount { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string Fee { get; set; }

        public string FromAddress { get; set; }

        public Guid OperationId { get; set; }

        public string SignedTxData { get; set; }
        
        public string ToAddress { get; set; }

        public string TxHash { get; set; }
    }
}
