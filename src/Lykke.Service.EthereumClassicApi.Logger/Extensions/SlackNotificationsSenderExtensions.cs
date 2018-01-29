using System.Threading.Tasks;
using Akka.Event;
using Lykke.Service.EthereumClassicApi.Logger.Serialization;
using Lykke.SlackNotifications;
using Newtonsoft.Json;

namespace Lykke.Service.EthereumClassicApi.Logger.Extensions
{
    internal static class SlackNotificationsSenderExtensions
    {
        public static async Task NotifyAboutEventAsync(this ISlackNotificationsSender sender, LogEvent logEvent)
        {
            var message = logEvent.Message.ToString();

            switch (logEvent)
            {
                case Info _:
                    await sender.SendInfoAsync(message);
                    break;
                case Warning _:
                    await sender.SendWarningAsync(message);
                    break;
                case Error _:
                    await sender.SendErrorAsync(message);
                    break;
            }
        }

        public static async Task NotifyAboutEventAsync(this ISlackNotificationsSender sender, LykkeLogEvent logEvent)
        {
            var message = BuildMessage(logEvent);

            switch (logEvent)
            {
                case LykkeInfo _:
                    await sender.SendInfoAsync(message);
                    break;
                case LykkeWarning _:
                    await sender.SendWarningAsync(message);
                    break;
                case LykkeError _:
                case LykkeFatalError _:
                    await sender.SendErrorAsync(message);
                    break;
                case LykkeMonitoring _:
                    await sender.SendMonitorAsync(message);
                    break;
            }
        }

        private static string BuildMessage(LykkeLogEvent logEvent)
        {
            var notification = new LykkeNotification
            {
                Component = logEvent.LogSource,
                Context = new LykkeLogContext
                {
                    Thread = logEvent.Thread.ManagedThreadId.ToString().PadLeft(4, '0'),
                    Trigger = logEvent.Trigger
                },
                Info = logEvent.Message,
                Process = logEvent.Process
            };

            return JsonConvert.SerializeObject(notification, Formatting.Indented, new ActorRefConverter());
        }
    }
}
