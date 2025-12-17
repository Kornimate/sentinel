using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentinel.Models.Options.Interfaces;
using Sentinel.Services.LogWriters;
using Sentinel.Services.LogWriters.Interfaces;
using static System.Net.WebRequestMethods;

namespace Sentinel.Models.Options
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

        public ILogWriterOptions WithLoggedClassFilter(string filter)
        {
            _logWriter.SetFilter(filter);

            return this;
        }

        public ILogWriterOptions WithMinimumLogLevel(LogLevel logLevel)
        {
            _logWriter.SetMinimiumLogLevel(logLevel);

            return this;
        }

        public ILogWriterOptions WithLogContainerSize(int size)
        {
            _logWriter.SetLogContainerSize(size);

            return this;
        }
    }
}
