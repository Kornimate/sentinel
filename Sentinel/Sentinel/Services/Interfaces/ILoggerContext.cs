using Sentinel.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.Interfaces
{
    internal interface ILoggerContext
    {
        void AddLogWriter(ILogWriter logWriter);
        void RaiseNewLogEntryEvent(ILogEntry logEntry);
    }
}
