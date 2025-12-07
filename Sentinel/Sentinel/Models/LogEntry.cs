using Sentinel.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models
{
    public sealed class LogEntry : ILogEntry
    {
        public string Text => throw new NotImplementedException();

        public DateTime TimeStamp => throw new NotImplementedException();

        public LogLevel LogLevel => throw new NotImplementedException();

        public string Serialize() => String.Empty;
    }
}
