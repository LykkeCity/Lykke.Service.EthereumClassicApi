using System;
using Akka.Event;

namespace Lykke.Service.EthereumClassic.Api.Logger
{
    public class LykkeBusLogging : ILykkeLoggingAdapter
    {
        private readonly LoggingBus _bus;
        private readonly Type       _logClass;
        private readonly string     _logSource;


        public LykkeBusLogging(
            LoggingBus bus,
            string logSource,
            Type logClass)
        {
            _bus       = bus;
            _logSource = logSource;
            _logClass  = logClass;
        }


        public void Info(string message, long? duration = null, string process = "", object trigger = null)
        {
            _bus.Publish(new LykkeInfo
            (
                logSource: _logSource,
                logClass:  _logClass,
                message:   message,
                duration:  duration,
                process:   process,
                trigger:   trigger
            ));
        }

        public void Warning(string message, long? duration = null, string process = "", object trigger = null, Exception cause = null)
        {
            _bus.Publish(new LykkeWarning
            (
                logSource: _logSource,
                logClass:  _logClass,
                message:   message,
                duration:  duration,
                process:   process,
                trigger:   trigger,
                cause:     cause
            ));
        }

        public void Error(string message, long? duration = null, string process = "", object trigger = null, Exception cause = null)
        {
            _bus.Publish(new LykkeError
            (
                logSource: _logSource,
                logClass:  _logClass,
                message:   message,
                duration:  duration,
                process:   process,
                trigger:   trigger,
                cause:     cause
            ));
        }

        public void FatalError(string message, long? duration = null, string process = "", object trigger = null, Exception cause = null)
        {
            _bus.Publish(new LykkeFatalError
            (
                logSource: _logSource,
                logClass:  _logClass,
                message:   message,
                duration:  duration,
                process:   process,
                trigger:   trigger,
                cause:     cause
            ));
        }

        public void Monitoring(string message, long? duration = null, string process = "", object trigger = null)
        {
            _bus.Publish(new LykkeMonitoring
            (
                logSource: _logSource,
                logClass:  _logClass,
                message:   message,
                duration:  duration,
                process:   process,
                trigger:   trigger
            ));
        }
    }
}