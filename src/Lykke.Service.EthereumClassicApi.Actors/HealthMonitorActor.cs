using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public class HealthMonitorActor : ReceiveActor
    {
        private readonly IHealthMonitorRole _healthMonitorRole;


        public HealthMonitorActor(
            IHealthMonitorRole healthMonitorRole)
        {
            _healthMonitorRole = healthMonitorRole;

            ReceiveAsync<UpdateHealthStatus>(
                ProcessMessageAsync);
        }


        private async Task ProcessMessageAsync(UpdateHealthStatus message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    await _healthMonitorRole.UpdateHealthStatusAsync();
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
        }
    }
}
