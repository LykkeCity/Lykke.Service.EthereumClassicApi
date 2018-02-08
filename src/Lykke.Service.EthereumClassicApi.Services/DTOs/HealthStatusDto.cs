using System;

namespace Lykke.Service.EthereumClassicApi.Services.DTOs
{
    public class HealthStatusDto
    {
        public string ApplicationName { get; set; }

        public string ApplicationVersion { get; set; }
        
        public string EnvironmentInfo { get; set; }
        
        public bool IsDebug { get; set; }
    }
}
