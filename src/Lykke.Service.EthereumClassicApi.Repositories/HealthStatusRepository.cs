using System;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class HealthStatusRepository : IHealthStatusRepository
    {
        private HealthStatusDto _healthStatus;


        public HealthStatusRepository()
        {
            _healthStatus = new HealthStatusDto
            {
                ApplicationName    = Constants.ApplicationName,
                ApplicationVersion = PlatformServices.Default.Application.ApplicationVersion,
                EnvironmentInfo    = Environment.GetEnvironmentVariable("ENV_INFO"),
                IsDebug            = Constants.IsDebug,
                IsHealthy          = true
            };
        }

        public HealthStatusDto Get()
        {
            return _healthStatus;
        }

        public void Update(HealthStatusDto dto)
        {
            _healthStatus = dto;
        }
    }
}
