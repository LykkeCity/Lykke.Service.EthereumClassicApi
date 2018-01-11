using System;
using System.Threading.Tasks;
using Akka.Event;
using Common.Log;
using Lykke.Service.EthereumClassic.Api.Logger.Serialization;
using Newtonsoft.Json;

namespace Lykke.Service.EthereumClassic.Api.Logger.Extensions
{
    internal static class LogExtensions
    {
        public static async Task LogEventAsync(this ILog log, LogEvent logEvent)
        {
            var component = logEvent.LogSource;
            var dateTime  = logEvent.Timestamp;
            var info      = logEvent.Message.ToString();

            switch (logEvent)
            {
                case Debug _:
                case Info  _:
                    await log.WriteInfoAsync
                    (
                        component: component,
                        process:   "",
                        context:   "",
                        info:      info,
                        dateTime:  dateTime
                    );

                    break;
                case Warning _:
                    await log.WriteWarningAsync
                    (
                        component: component,
                        process:   "",
                        context:   "",
                        info:      info,
                        dateTime:  dateTime
                    );

                    break;
                case Error error:
                    await log.WriteErrorAsync
                    (
                        component: component,
                        process:   "",
                        context:   "",
                        exception: new Exception(info, error.Cause),
                        dateTime:  dateTime
                    );

                    break;
            }
        }

        public static async Task LogEventAsync(this ILog log, LykkeLogEvent logEvent)
        {
            var component = logEvent.LogSource;
            var context   = BuildContext(logEvent);
            var dateTime  = logEvent.Timestamp;
            var info      = logEvent.Message;
            var process   = logEvent.Process;


            switch (logEvent)
            {
                case LykkeInfo _:
                    await log.WriteInfoAsync
                    (
                        component: component,
                        process:   process,
                        context:   context,
                        info:      info,
                        dateTime:  dateTime
                    );

                    break;
                case LykkeWarning warning:
                    await log.WriteWarningAsync
                    (
                        component: component,
                        process:   process,
                        context:   context,
                        info:      info,
                        ex:        warning.Cause,
                        dateTime:  dateTime
                    );

                    break;
                case LykkeError error:
                    await log.WriteErrorAsync
                    (
                        component: component,
                        process:   process,
                        context:   context,
                        exception: WrapException(info, error.Cause),
                        dateTime:  dateTime
                    );

                    break;
                case LykkeFatalError fatalError:
                    await log.WriteFatalErrorAsync
                    (
                        component: component,
                        process:   process,
                        context:   context,
                        exception: WrapException(info, fatalError.Cause),
                        dateTime:  dateTime
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
                Thread   = logEvent.Thread.ManagedThreadId.ToString().PadLeft(4, '0'),
                Trigger  = logEvent.Trigger
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