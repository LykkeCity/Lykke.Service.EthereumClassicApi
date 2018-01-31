using System;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Services.DTOs;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.EthereumClassicApi.Services
{
    public class HealthService : IHealthService
    {
        public HealthStatusDto GetHealthStatus()
        {
            return new HealthStatusDto
            {
                ApplicationName = Constants.ApplicationName,
                ApplicationVersion = PlatformServices.Default.Application.ApplicationVersion,
                EnvironmentInfo = Environment.GetEnvironmentVariable("ENV_INFO"),
                IsDebug = Constants.IsDebug
            };
        }
    }
}
