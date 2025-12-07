using Sentinel.Models.Interfaces;
using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models
{
    public sealed class LoggerBuilderOptions : ILoggerBuilderOptions
    {
        private readonly IEnumerable<ILogWriter> _logWriters;

        public LoggerBuilderOptions()
        {
            _logWriters = [];
        }

        public ILoggerBuilderOptions AddConsoleLogger(Action<ILogWriterOptions> writerOptions)
        {
            throw new NotImplementedException();
        }

        public ILoggerBuilderOptions AddCustomLogger(Action<ILogWriterOptions> writerOptions)
        {
            throw new NotImplementedException();
        }

        public ILoggerBuilderOptions AddJsonLogger(Action<ILogWriterOptions> writerOptions)
        {
            throw new NotImplementedException();
        }

        public ILoggerBuilderOptions AddXmlLogger(Action<ILogWriterOptions> writerOptions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ILogWriter> GetRegisteredLogWrites()
        {
            throw new NotImplementedException();
        }
    }
}
