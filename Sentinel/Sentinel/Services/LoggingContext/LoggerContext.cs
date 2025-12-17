using Sentinel.Models.LogTypes.Interfaces;
using Sentinel.Services.HelathChecks;
using Sentinel.Services.HelathChecks.Interfaces;
using Sentinel.Services.LoggingContext.Interfaces;
using Sentinel.Services.LogWriters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.LoggingContext
{
    public sealed class LoggerContext : ILoggerContext
    {
        private static ILoggerContext? _instance;

        private event EventHandler<ILogEntry>? LogCreatedEvent;
        private IHealthCheckService _healthCheckService;
        private IList<ILogWriter> _logWriters;

        private LoggerContext()
        {
            _healthCheckService = new HealthCheckService();
            _logWriters = [];
        }

        internal static ILoggerContext GetInstance()
        {
            return _instance ??= new LoggerContext();
        }

        public void AddLogWriter(ILogWriter logWriter)
        {
            LogCreatedEvent += logWriter.AddLogMessage;

            _logWriters.Add(logWriter);

            var loggerTask = logWriter.GetBackgroundConsumerTask();

            _healthCheckService.AddTaskToCheck(logWriter, loggerTask is null ? logWriter.StartNewBackgroundTask() : loggerTask);
        }

        public void RaiseNewLogEntryEvent(ILogEntry logEntry)
        {
            LogCreatedEvent?.Invoke(this, logEntry);
        }

        public void ShutDown()
        {
            _healthCheckService?.ShutDown();

            foreach (var logWriter in _logWriters)
            {
                logWriter?.ShutDown();
            }
        }
    }
}
