using System;

namespace Lykke.Service.EthereumClassicApi.Repositories.DTOs
{
    public class HealthStatusDto
    {
        public string ApplicationName { get; set; }

        public string ApplicationVersion { get; set; }

        public double CashinApdex { get; set; }

        public double CashoutApdex { get; set; }

        public string EnvironmentInfo { get; set; }

        public bool EthereumRpcNodeStatusIsAlive { get; set; }

        public bool IsDebug { get; set; }

        public bool IsHealthy { get; set; }

        public bool SignServiceIsAlive { get; set; }

        public string StatusMessage { get; set; }

        public TimeSpan Uptime { get; set; }
    }
}
