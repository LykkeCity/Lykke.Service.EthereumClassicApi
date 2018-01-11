using System;

namespace Lykke.Service.EthereumClassic.Api.Common.Settings
{
    public class EthereumClassicApiSettings
    {
        public TimeSpan BalancesCheckInterval { get; set; }

        public TimeSpan TransactionApdexPeriod { get; set; }

        public int TransactionApdexToleratingCount { get; set; }

        public int TransactionConfirmationLevel { get; set; }
        
        public DbSettings Db { get; set; }

        public string DefaultMaxGasPrice { get; set; }

        public string DefaultMinGasPrice { get; set; }

        public string EthereumRpcNodeType { get; set; }

        public string EthereumRpcNodeUrl { get; set; }
        
        public int NrOfBalanceReaders { get; set; }
        
        public int NrOfOperationMonitors { get; set; }

        public int NrOfTransactionProcessors { get; set; }
    }
}
