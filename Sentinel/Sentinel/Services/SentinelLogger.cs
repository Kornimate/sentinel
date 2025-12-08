using Sentinel.Models;
using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services
{
    public sealed class SentinelLogger : ISentinelLogger
    {
        private ILoggerContext _context = LoggerContext.GetInstance();

        public void Log(LogLevel logLevel, string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(new LogEntry(message, logLevel, exception));
        }

        public void LogDebug(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(new LogEntry(message, LogLevel.DEBUG, exception));
        }

        public void LogError(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(new LogEntry(message, LogLevel.ERROR, exception));
        }

        public void LogFatal(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(new LogEntry(message, LogLevel.FATAL, exception));
        }

        public void LogInformation(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(new LogEntry(message, LogLevel.INFORMATION, exception));
        }

        public void LogVerbose(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(new LogEntry(message, exception: exception));
        }

        public void LogWarning(string message, Exception? exception = null)
        {
            _context.RaiseNewLogEntryEvent(new LogEntry(message, LogLevel.WARNING, exception));
        }
    }
}
