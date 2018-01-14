using System;
using System.Diagnostics;
using Akka.Actor;
using Lykke.Logs;
using Lykke.Service.EthereumClassicApi.Logger;

namespace Lykke.Service.EthereumClassicApi.Actors.Utils
{
    public class LykkeLoggerAdapter<T> : IDisposable
    {
        private readonly IUntypedActorContext _context;
        private readonly T                    _trigger;
        private readonly string               _process;
        private readonly Stopwatch            _stopwatch;


        private string    _customMessage;
        private Exception _exception;
        private LogLevel  _logLevel;
        private bool      _supressed;

        public LykkeLoggerAdapter(IUntypedActorContext context, string process, T trigger)
        {
            _context   = context;
            _logLevel  = LogLevel.Info;
            _process   = string.IsNullOrEmpty(process) ? typeof(T).Name : process;
            _stopwatch = new Stopwatch();
            _trigger   = trigger;

            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();

            if (!_supressed)
            {
                var duration    = _stopwatch.ElapsedMilliseconds;
                var lykkeLogger = _context.GetLykkeLogger();
                
                switch (_logLevel)
                {
                    case LogLevel.Info:
                        lykkeLogger.Info("", duration, _process, _trigger);
                        break;
                    case LogLevel.Warning:
                        lykkeLogger.Warning("", duration, _process, _trigger, _exception);
                        break;
                    case LogLevel.Error:
                        lykkeLogger.Error("", duration, _process, _trigger, _exception);
                        break;
                    case LogLevel.FatalError:
                        lykkeLogger.FatalError("", duration, _process, _trigger, _exception);
                        break;
                    case LogLevel.Monitoring:
                        lykkeLogger.Monitoring("", duration, _process, _trigger);
                        break;
                }
            }
        }

        public void Error(Exception e)
        {
            _exception = e;
            _logLevel  = LogLevel.Error;
        }

        public void SetMessage(string message)
        {
            _customMessage = message;
        }

        public void Suppress()
        {
            _supressed = true;
        }
    }
}
