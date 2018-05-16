using System;
using System.Net;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("api/isalive")]
    public class IsAliveController : Controller
    {
        private readonly IHealthService _healthService;

        public IsAliveController(
            IHealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet]
        public IActionResult GetHealthStatus()
        {
            try
            {
                var healthStatus = _healthService.GetHealthStatus();

                return Ok(new IsAliveResponse
                {
                    Env = healthStatus.EnvironmentInfo,
                    IsDebug = healthStatus.IsDebug,
                    Name = healthStatus.ApplicationName,
                    Version = healthStatus.ApplicationVersion
                });
            }
            catch (Exception)
            {
                return StatusCode
                (
                    (int)HttpStatusCode.InternalServerError,
                    ErrorResponse.Create($"Service is unhealthy")
                );
            }
        }
    }
}
