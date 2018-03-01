using System;
using System.Numerics;
using Lykke.Service.EthereumClassicApi.Common;

namespace Lykke.Service.EthereumClassicApi.Services.DTOs
{
    public class TransactionStateDto
    {
        public BigInteger? BlockNumber { get; set; }

        public DateTime? CompletedOn { get; set; }

        public string Error { get; set; }

        public TransactionState State { get; set; }
    }
}
