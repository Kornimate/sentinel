using Sentinel.Models.LogTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models.LogTypes
{
    public sealed record LogEntry : ILogEntry
    {
        public static ILogEntry CreateLogEntry(string message, string caller = "", LogLevel logLevel = LogLevel.VERBS, Exception? exception = null)
        {
            return new LogEntry()
            {
                Text = message,
                Level = logLevel,
                Exception = exception,
                FilterData = caller,
                TimeStamp = DateTime.UtcNow,
            };
        }
        public string Text { get; private set; } = string.Empty;

        public string FilterData { get; private set; } = string.Empty;

        public DateTime TimeStamp { get; private set; }

        public Exception? Exception { get; private set; }

        public LogLevel Level { get; private set; }

        public string Serialize()
        {
            return $"{TimeStamp:yyyy-MM-dd'T'HH:mm:ss.fff'Z'} [{Level}]: {Text}{(Exception is null ? "" : ", " + Exception.ToString())}";
        }
    }
}
