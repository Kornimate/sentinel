using Sentinel.Models.Interfaces;
using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services
{
    public sealed class JsonLogWriter : ILogWriter
    {
        public void AddLogMessage(object? sender, ILogEntry e)
        {
            throw new NotImplementedException();
        }
    }
}
