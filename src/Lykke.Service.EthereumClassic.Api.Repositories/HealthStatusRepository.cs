using System;
using Lykke.Service.EthereumClassic.Api.Common;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.EthereumClassic.Api.Repositories
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
                IsDebug            = Constants.IsDebug
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
