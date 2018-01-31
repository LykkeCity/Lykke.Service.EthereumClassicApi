using System;
using Lykke.Service.EthereumClassicApi.Common;

namespace Lykke.Service.EthereumClassicApi.Services.DTOs
{
    public class TransactionStateDto
    {
        public string Error { get; set; }
        
        public TransactionState State { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
