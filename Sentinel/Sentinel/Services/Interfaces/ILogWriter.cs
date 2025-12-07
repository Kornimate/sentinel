using Sentinel.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.Interfaces
{
    public interface ILogWriter
    {
        void AddLogMessage(object? sender, ILogEntry e);
    }
}
