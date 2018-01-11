using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassic.Api.Actors.Extensions;
using Lykke.Service.EthereumClassic.Api.Actors.Messages;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;


namespace Lykke.Service.EthereumClassic.Api.Actors
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
