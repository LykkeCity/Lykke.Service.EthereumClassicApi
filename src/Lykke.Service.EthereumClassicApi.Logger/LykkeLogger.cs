using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch;
using Akka.Event;
using Autofac;
using Common.Log;
using Lykke.Service.EthereumClassicApi.Logger.Extensions;
using Lykke.SlackNotifications;


namespace Lykke.Service.EthereumClassicApi.Logger
{
    public class LykkeLogger : ReceiveActor, IRequiresMessageQueue<ILoggerMessageQueueSemantics>
    {
        private static IContainer _container;

        public static void Configure(IContainer container)
        {
            _container = container;
        }

        private readonly ILog                      _lykkeLog;
        private readonly ISlackNotificationsSender _lykkeNotificationsSender;


        public LykkeLogger()
        {
            if (_container == null)
            {
                throw new InvalidOperationException($"{nameof(LykkeLogger)} {nameof(Configure)} method should be called before actor system will be created.");
            }

            _lykkeLog                 = _container.Resolve<ILog>();
            _lykkeNotificationsSender = _container.Resolve<ISlackNotificationsSender>();


            Receive<InitializeLogger>(
                msg => ProcessMessage(msg));

            ReceiveAsync<LogEvent>(
                ProcessMessageAsync);
            
            SubscribeAndReceiveAsync<LykkeLogEvent>(
                ProcessMessageAsync);
        }


        private void ProcessMessage(InitializeLogger message)
        {
            Sender.Tell(new LoggerInitialized());
        }
        
        private async Task ProcessMessageAsync(LogEvent message)
        {
            await Task.WhenAll
            (
                _lykkeLog.LogEventAsync(message),
                _lykkeNotificationsSender.NotifyAboutEventAsync(message)
            );
        }

        private async Task ProcessMessageAsync(LykkeLogEvent message)
        {
            await Task.WhenAll
            (
                _lykkeLog.LogEventAsync(message),
                _lykkeNotificationsSender.NotifyAboutEventAsync(message)
            );
        }

        private void SubscribeAndReceiveAsync<T>(Func<T, Task> handler)
        {
            Context.System.EventStream.Subscribe<T>(Self);

            ReceiveAsync(handler);
        }
    }
}