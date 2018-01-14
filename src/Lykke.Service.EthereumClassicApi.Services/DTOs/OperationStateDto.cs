using System;
using System.Numerics;

namespace Lykke.Service.EthereumClassicApi.Services.DTOs
{
    public class OperationStateDto
    {
        public BigInteger Amount { get; set; }

        public string Error { get; set; }

        public string FromAddress { get; set; }

        public Guid OperationId { get; set; }

        public string ToAddress { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string TxHash { get; set; }
    }
}
