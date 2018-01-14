using System.Net;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("api/isalive")]
    public class IsAliveController : Controller
    {
        private readonly IHealthStatusRepository _healthStatusRepository;

        public IsAliveController(
            IHealthStatusRepository healthStatusRepository)
        {
            _healthStatusRepository = healthStatusRepository;
        }

        [HttpGet]
        public IActionResult GetHealthStatus()
        {
            var healthStatus = _healthStatusRepository.Get();

            if (healthStatus.IsHealthy)
            {
                return Ok(new IsAliveResponse
                {
                    Env     = healthStatus.EnvironmentInfo,
                    IsDebug = healthStatus.IsDebug,
                    Name    = healthStatus.ApplicationName,
                    Version = healthStatus.ApplicationVersion
                });
            }
            else
            {
                return StatusCode
                (
                    (int) HttpStatusCode.InternalServerError,
                    ErrorResponse.Create($"Service is unhealthy: {healthStatus.StatusMessage}")
                );
            }
        }
    }
}
