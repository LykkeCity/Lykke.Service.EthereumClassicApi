using System;

namespace Lykke.Service.EthereumClassicApi.Common.Settings
{
    public class EthereumClassicApiSettings
    {
        public TimeSpan BalancesCheckInterval { get; set; }

        public DbSettings Db { get; set; }

        public string DefaultMaxGasPrice { get; set; }

        public string DefaultMinGasPrice { get; set; }

        public string EthereumRpcNodeType { get; set; }

        public string EthereumRpcNodeUrl { get; set; }

        public int NrOfBalanceObservers { get; set; }

        public int NrOfTransactionMonitors { get; set; }
        
        public int TransactionConfirmationLevel { get; set; }

        public TimeSpan TransactionStatesCheckInterval { get; set; }
    }
}
