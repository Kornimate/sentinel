using Sentinel.Models;
using Sentinel.Models.LogTypes;
using Sentinel.Services.Logger.Interfaces;
using Sentinel.Services.LoggingContext;
using Sentinel.Services.LoggingContext.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.Logger
{
    public sealed class SentinelLogger<T> : ISentinelLogger<T> where T : class
    {
        private ILoggerContext _context = LoggerContext.GetInstance();

        public void Log(LogLevel logLevel, string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(LogEntry.CreateLogEntry(message, typeof(T).FullName!, logLevel, exception));
        }

        public void LogDebug(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(LogEntry.CreateLogEntry(message, typeof(T).FullName!, LogLevel.DEBUG, exception));
        }

        public void LogError(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(LogEntry.CreateLogEntry(message, typeof(T).FullName!, LogLevel.ERROR, exception));
        }

        public void LogFatal(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(LogEntry.CreateLogEntry(message, typeof(T).FullName!, LogLevel.FATAL, exception));
        }

        public void LogInformation(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(LogEntry.CreateLogEntry(message, typeof(T).FullName!, LogLevel.INFOR, exception));
        }

        public void LogVerbose(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(LogEntry.CreateLogEntry(message, caller: typeof(T).FullName!, exception: exception));
        }

        public void LogWarning(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(LogEntry.CreateLogEntry(message, typeof(T).FullName!, LogLevel.WARNG, exception));
        }

        public void ShutDown()
        {
            _context.ShutDown();
        }
    }
}
