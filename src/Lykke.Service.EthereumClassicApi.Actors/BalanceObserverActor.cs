using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Common.Chaos;
using Lykke.Service.EthereumClassicApi.Actors.Extensions;
using Lykke.Service.EthereumClassicApi.Actors.Messages;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public class BalanceObserverActor : ReceiveActor
    {
        private readonly IBalanceObserverRole _balanceObserverRole;
        private readonly IChaosKitty _chaosKitty;

        public BalanceObserverActor(
            IBalanceObserverRole balanceObserverRole,
            IChaosKitty chaosKitty)
        {
            _balanceObserverRole = balanceObserverRole;
            _chaosKitty = chaosKitty;

            ReceiveAsync<CheckBalance>(
                ProcessMessageAsync);
        }


        private async Task ProcessMessageAsync(CheckBalance message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    _chaosKitty.Meow(message.Address);
                    var balance = await _balanceObserverRole.UpdateBalanceAsync(message.Address, message.BlockNumber);

                    if (balance > 0)
                    {
                        logger.Info($"{message.Address} balance amount is {balance}");
                    }
                    else
                    {
                        logger.Suppress();
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
                finally
                {
                    Sender.Tell(BalanceChecked.Instance);
                }
            }
        }
    }
}
