using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public class BalanceObserverManagerActor : ReceiveActor
    {
        private readonly IBalanceObserverManagerRole _balanceObserverManagerRole;


        public BalanceObserverManagerActor(
            IBalanceObserverManagerRole balanceObserverManagerRole)
        {
            _balanceObserverManagerRole = balanceObserverManagerRole;


            ReceiveAsync<BeginBalanceMonitoring>(
                ProcessMessageAsync);

            ReceiveAsync<EndBalanceMonitoring>(
                ProcessMessageAsync);
        }


        private async Task ProcessMessageAsync(BeginBalanceMonitoring message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    await _balanceObserverManagerRole.BeginBalanceMonitoringAsync(message.Address);

                    Sender.Tell(new Status.Success(null));
                }
                catch (Exception e)
                {
                    Sender.Tell(new Status.Failure
                    (
                        e
                    ));

                    logger.Error(e);
                }
            }
        }

        private async Task ProcessMessageAsync(EndBalanceMonitoring message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    await _balanceObserverManagerRole.EndBalanceMonitoringAsync(message.Address);

                    Sender.Tell(new Status.Success(null));
                }
                catch (Exception e)
                {
                    Sender.Tell(new Status.Failure
                    (
                        e
                    ));

                    logger.Error(e);
                }
            }
        }
    }
}
