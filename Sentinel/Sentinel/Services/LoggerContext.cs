using Sentinel.Models.Interfaces;
using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services
{
    public sealed class LoggerContext : ILoggerContext
    {
        private static ILoggerContext? _instance;

        private static int _loggerIndexer = 0;

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

            CheckIfLogWriterConfiguredCorrectly(logWriter);

            _logWriters.Add(logWriter);

            var loggerTask = logWriter.GetBackgroundConsumerTask();

            _healthCheckService.AddTaskToCheck(logWriter, loggerTask is null ? logWriter.StartNewBackgroundTask() : loggerTask);
        }

        public void RaiseNewLogEntryEvent(ILogEntry logEntry)
        {
            LogCreatedEvent?.Invoke(this, logEntry);
        }

        private void CheckIfLogWriterConfiguredCorrectly(ILogWriter newLogWriter)
        {
            foreach (ILogWriter existingLogWriter in _logWriters)
            {
                if (!newLogWriter.WriteToConsole()
                    && !existingLogWriter.WriteToConsole()
                    && newLogWriter.GetFileName() != null
                    && newLogWriter.GetFileName() == existingLogWriter.GetFileName()
                    && newLogWriter.GetSubDirectory() == existingLogWriter.GetSubDirectory()
                    && newLogWriter.GetFilePath() == existingLogWriter.GetFilePath())
                {
                    throw new ArgumentException("Loggers can not write the same file (same path, subdirectory and filename)");
                }
            }
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
