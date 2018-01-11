using System;
using System.Threading.Tasks;
using Akka.Actor;
using Lykke.Service.EthereumClassic.Api.Actors.Extensions;
using Lykke.Service.EthereumClassic.Api.Actors.Messages;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Actors
{
    public class BalanceObserverActor : ReceiveActor
    {
        private readonly IBalanceObserverRole _balanceObserverRole;


        public BalanceObserverActor(
            IBalanceObserverRole balanceObserverRole)
        {
            _balanceObserverRole = balanceObserverRole;


            ReceiveAsync<CheckBalance>(
                ProcessMessageAsync);
        }


        private async Task ProcessMessageAsync(CheckBalance message)
        {
            using (var logger = Context.GetLogger(message))
            {
                try
                {
                    var balance = await _balanceObserverRole.GetBalanceAsync(message.Address, message.BlockNumber);

                    if (balance > 0)
                    {
                        // TODO: Add balance to the log message
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
