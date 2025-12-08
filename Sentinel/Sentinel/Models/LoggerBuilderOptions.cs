using Sentinel.Models.Interfaces;
using Sentinel.Services;
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
        private readonly IList<ILogWriter> _logWriters;
        private ILogWriterOptions? _logWriterOptions;

        public LoggerBuilderOptions()
        {
            _logWriters = [];
            _logWriterOptions = null;
        }

        public ILoggerBuilderOptions AddConsoleLogger(Action<ILogWriterOptions> writerOptions)
        {
            _logWriterOptions = new LogWriterOptions(new ConsoleLogWriter());

            writerOptions(_logWriterOptions);

            _logWriters.Add(_logWriterOptions.GetWriterInstance());

            return this;
        }


        public ILoggerBuilderOptions AddJsonLogger(Action<ILogWriterOptions> writerOptions)
        {
            _logWriterOptions = new LogWriterOptions(new JsonLogWriter());

            writerOptions(_logWriterOptions);

            _logWriters.Add(_logWriterOptions.GetWriterInstance());

            return this;
        }

        public ILoggerBuilderOptions AddXmlLogger(Action<ILogWriterOptions> writerOptions)
        {
            _logWriterOptions = new LogWriterOptions(new JsonLogWriter());

            writerOptions(_logWriterOptions);

            _logWriters.Add(_logWriterOptions.GetWriterInstance());

            return this;
        }
        public ILoggerBuilderOptions AddCustomLogger<T>(Action<ILogWriterOptions> writerOptions) where T : ILogWriter
        {
            _logWriterOptions = new LogWriterOptions(new JsonLogWriter());

            writerOptions(_logWriterOptions);

            _logWriters.Add(_logWriterOptions.GetWriterInstance());

            return this;
        }

        public IEnumerable<ILogWriter> GetRegisteredLogWrites()
        {
            return _logWriters;
        }
    }
}
