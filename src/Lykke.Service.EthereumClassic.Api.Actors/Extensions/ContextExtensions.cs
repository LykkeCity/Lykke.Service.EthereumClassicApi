using Akka.Actor;
using Lykke.Service.EthereumClassic.Api.Actors.Utils;

namespace Lykke.Service.EthereumClassic.Api.Actors.Extensions
{
    public static class ContextExtensions
    {
        public static LykkeLoggerAdapter<T> GetLogger<T>(this IUntypedActorContext context, T message, string process = "")
        {
            return new LykkeLoggerAdapter<T>(context, process, message);
        }
    }
}
