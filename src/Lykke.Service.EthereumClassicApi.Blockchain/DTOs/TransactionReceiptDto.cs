using System.Numerics;

namespace Lykke.Service.EthereumClassicApi.Blockchain.DTOs
{
    public class TransactionReceiptDto
    {
        public string BlockHash { get; set; }

        public BigInteger BlockNumber { get; set; }

        public string ContractAddress { get; set; }

        public BigInteger CumulativeGasUsed { get; set; }

        public BigInteger GasUsed { get; set; }

        public BigInteger Status { get; set; }
        public string TransactionHash { get; set; }

        public BigInteger TransactionIndex { get; set; }
    }
}
