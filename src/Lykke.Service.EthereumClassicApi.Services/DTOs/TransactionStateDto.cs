using System;
using Lykke.Service.EthereumClassicApi.Common;

namespace Lykke.Service.EthereumClassicApi.Services.DTOs
{
    public class TransactionStateDto
    {
        public DateTime? CompletedOn { get; set; }

        public string Error { get; set; }

        public TransactionState State { get; set; }
    }
}
