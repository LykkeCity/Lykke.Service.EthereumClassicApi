using System;
using System.Threading.Tasks;
using Akka.Event;
using Common.Log;
using Lykke.Service.EthereumClassicApi.Logger.Serialization;
using Newtonsoft.Json;

namespace Lykke.Service.EthereumClassicApi.Logger.Extensions
{
    internal static class LogExtensions
    {
        public static async Task LogEventAsync(this ILog log, LogEvent logEvent)
        {
            var component = logEvent.LogSource;
            var dateTime = logEvent.Timestamp;
            var info = logEvent.Message.ToString();

            switch (logEvent)
            {
                case Debug _:
                case Info _:
                    await log.WriteInfoAsync
                    (
                        component,
                        "",
                        "",
                        info,
                        dateTime
                    );

                    break;
                case Warning _:
                    await log.WriteWarningAsync
                    (
                        component,
                        "",
                        "",
                        info,
                        dateTime
                    );

                    break;
                case Error error:
                    await log.WriteErrorAsync
                    (
                        component,
                        "",
                        "",
                        new Exception(info, error.Cause),
                        dateTime
                    );

                    break;
            }
        }

        public static async Task LogEventAsync(this ILog log, LykkeLogEvent logEvent)
        {
            var component = logEvent.LogSource;
            var context = BuildContext(logEvent);
            var dateTime = logEvent.Timestamp;
            var info = logEvent.Message;
            var process = logEvent.Process;


            switch (logEvent)
            {
                case LykkeInfo _:
                    await log.WriteInfoAsync
                    (
                        component,
                        process,
                        context,
                        info,
                        dateTime
                    );

                    break;
                case LykkeWarning warning:
                    await log.WriteWarningAsync
                    (
                        component,
                        process,
                        context,
                        info,
                        warning.Cause,
                        dateTime
                    );

                    break;
                case LykkeError error:
                    await log.WriteErrorAsync
                    (
                        component,
                        process,
                        context,
                        WrapException(info, error.Cause),
                        dateTime
                    );

                    break;
                case LykkeFatalError fatalError:
                    await log.WriteFatalErrorAsync
                    (
                        component,
                        process,
                        context,
                        WrapException(info, fatalError.Cause),
                        dateTime
                    );

                    break;
                case LykkeMonitoring _:
                    break;
            }
        }

        private static string BuildContext(LykkeLogEvent logEvent)
        {
            var context = new LykkeLogContext
            {
                Duration = logEvent.Duration,
                Thread = logEvent.Thread.ManagedThreadId.ToString().PadLeft(4, '0'),
                Trigger = logEvent.Trigger
            };

            return JsonConvert.SerializeObject(context, Formatting.None, new ActorRefConverter());
        }

        private static Exception WrapException(string info, Exception cause)
        {
            return string.IsNullOrEmpty(info)
                ? cause
                : new Exception(info, cause);
        }
    }
}
