using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassic.Api.Actors.Extensions;
using Lykke.Service.EthereumClassic.Api.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Actors.Messages;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Actors
{
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public class OperationMonitorDispatcherActor : ReceiveActor
    {
        private readonly IOperationMonitorDispatcherRole _operationMonitorDispatcherRole;
        private readonly IActorRef                       _operationMonitors;


        public OperationMonitorDispatcherActor(
            IOperationMonitorDispatcherRole operationMonitorDispatcherRole,
            IOperationMonitorsFactory operationMonitorsFactory)
        {
            _operationMonitorDispatcherRole = operationMonitorDispatcherRole;
            _operationMonitors              = operationMonitorsFactory.Build(Context, "operation-monitors");


            Receive<CheckOperationState>(
                msg => ProcessMessage(msg));

            ReceiveAsync<CheckOperationStates>(
                ProcessMessageAsync);


            Self.Tell
            (
                message: CheckOperationStates.Instance,
                sender:  Nobody.Instance
            );
        }


        private void ProcessMessage(CheckOperationState message)
        {
            _operationMonitors.Forward(message);
        }

        private async Task ProcessMessageAsync(CheckOperationStates message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var operationIds = await _operationMonitorDispatcherRole.GetAllOperationIdsAsync();

                    foreach (var operationId in operationIds)
                    {
                        _operationMonitors.Tell(new CheckOperationState
                        (
                            operationId: operationId
                        ));
                    }
                }
                catch (Exception e)
                {
                    Context.System.Scheduler.ScheduleTellOnce
                    (
                        delay:    TimeSpan.FromMinutes(1),
                        receiver: Self,
                        message:  message,
                        sender:   Nobody.Instance
                    );

                    logger.Error(e);
                }
            }
        }
    }
}
