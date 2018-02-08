using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Utils;

namespace Lykke.Service.EthereumClassicApi.Actors.Extensions
{
    public static class ContextExtensions
    {
        public static LykkeLogBuilder<T> GetLogger<T>(this IUntypedActorContext context, T message, string process = "")
        {
            return new LykkeLogBuilder<T>(context, process, message);
        }
    }
}
