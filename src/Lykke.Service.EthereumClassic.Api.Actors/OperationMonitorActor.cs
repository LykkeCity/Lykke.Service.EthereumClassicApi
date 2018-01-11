using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassic.Api.Actors.Extensions;
using Lykke.Service.EthereumClassic.Api.Actors.Messages;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Actors
{
    public class OperationMonitorActor : ReceiveActor
    {
        private readonly IOperationMonitorRole _operationMonitorRole;

        public OperationMonitorActor(
            IOperationMonitorRole operationMonitorRole)
        {
            _operationMonitorRole = operationMonitorRole;


            ReceiveAsync<CheckOperationState>(
                ProcessMessageAsync);
        }

        private async Task ProcessMessageAsync(CheckOperationState message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var operationCompleted = await _operationMonitorRole.CheckOperationAsync(message.OperationId);

                    if (!operationCompleted)
                    {
                        ScheduleRetry(message);
                    }

                    logger.Suppress();
                }
                catch (Exception e)
                {
                    ScheduleRetry(message);

                    logger.Error(e);
                }
            }
        }

        private void ScheduleRetry(CheckOperationState message)
        {
            // TODO: Load delay from config

            Context.System.Scheduler.ScheduleTellOnce
            (
                delay:    TimeSpan.FromSeconds(30),
                receiver: Self,
                message:  message,
                sender:   Sender
            );
        }
    }
}
