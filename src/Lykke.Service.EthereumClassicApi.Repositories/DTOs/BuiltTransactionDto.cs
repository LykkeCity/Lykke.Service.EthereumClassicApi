﻿using System;
using System.Numerics;
using Lykke.Service.EthereumClassicApi.Common;

namespace Lykke.Service.EthereumClassicApi.Repositories.DTOs
{
    public class BuiltTransactionDto
    {
        public BigInteger Amount { get; set; }

        public DateTime BuiltOn { get; set; }

        public BigInteger Fee { get; set; }

        public string FromAddress { get; set; }

        public BigInteger GasPrice { get; set; }

        public bool IncludeFee { get; set; }

        public BigInteger Nonce { get; set; }

        public Guid OperationId { get; set; }

        public TransactionState State { get; set; }

        public string ToAddress { get; set; }

        public string TxData { get; set; }
    }
}
