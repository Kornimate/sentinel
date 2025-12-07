using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentinel.Models.Interfaces;
using Sentinel.Services.Interfaces;

namespace Sentinel.Models
{
    internal sealed class LogWriterOptions : ILogWriterOptions
    {
        private ILogWriter _logWriter;
        public LogWriterOptions(ILogWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public ILogWriter GetWriterInstance()
        {
            return _logWriter;
        }
    }
}
