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
        private static ILoggerContext _instance;

        private event EventHandler<ILogEntry>? logCreatedEvent;

        private LoggerContext() { } // to ensure singleton
        internal static ILoggerContext GetInstance()
        {
            return _instance ??= new LoggerContext();
        }

        public void AddLogWriter(ILogWriter logWriter)
        {
            this.logCreatedEvent += logWriter.AddLogMessage;
        }

        public void RaiseNewLogEntryEvent(ILogEntry logEntry)
        {
            this.logCreatedEvent?.Invoke(this, logEntry);
        }
    }
}
