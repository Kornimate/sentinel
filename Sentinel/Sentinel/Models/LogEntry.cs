using Sentinel.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models
{
    public sealed record LogEntry : ILogEntry
    {
        public LogEntry(string message, string caller = "", LogLevel logLevel = LogLevel.VERBOSE, Exception? exception = null)
        {
            Text = message;
            Level = logLevel;
            Exception = exception; 
            FilterData = caller;
            TimeStamp = DateTime.UtcNow;
        }
        public string Text { get; private set; }

        public string FilterData { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public Exception? Exception { get; private set; }

        public LogLevel Level { get; private set; }

        public string Serialize() => String.Empty;
    }
}
