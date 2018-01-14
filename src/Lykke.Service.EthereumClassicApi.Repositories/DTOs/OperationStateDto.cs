using System;
using System.Numerics;

namespace Lykke.Service.EthereumClassicApi.Repositories.DTOs
{
    public class OperationStateDto
    {
        public BigInteger Amount { get; set; }

        public bool Completed { get; set; }

        public string Error { get; set; }

        public bool Failed { get; set; }

        public BigInteger Fee { get; set; }

        public string FromAddress { get; set; }

        public Guid OperationId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string ToAddress { get; set; }

        public string TxHash { get; set; }
    }
}
