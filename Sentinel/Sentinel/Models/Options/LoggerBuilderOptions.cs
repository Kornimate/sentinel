using Sentinel.Models.Options.Interfaces;
using Sentinel.Services;
using Sentinel.Services.LogWriters;
using Sentinel.Services.LogWriters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models.Options
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

            _logWriters.Add(_logWriterOptions.GetWriterInstance().Build());

            return this;
        }


        public ILoggerBuilderOptions AddJsonLogger(Action<ILogWriterOptions> writerOptions)
        {
            _logWriterOptions = new LogWriterOptions(new JsonLogWriter());

            writerOptions(_logWriterOptions);

            _logWriters.Add(_logWriterOptions.GetWriterInstance().Build());

            return this;
        }

        public ILoggerBuilderOptions AddXmlLogger(Action<ILogWriterOptions> writerOptions)
        {
            _logWriterOptions = new LogWriterOptions(new XmlLogWriter());

            writerOptions(_logWriterOptions);

            _logWriters.Add(_logWriterOptions.GetWriterInstance().Build());

            return this;
        }
        public ILoggerBuilderOptions AddCustomLogger<T>(Action<ILogWriterOptions> writerOptions) where T : ILogWriter
        {
            _logWriterOptions = new LogWriterOptions(Activator.CreateInstance<T>());

            writerOptions(_logWriterOptions);

            _logWriters.Add(_logWriterOptions.GetWriterInstance().Build());

            return this;
        }

        public IEnumerable<ILogWriter> GetRegisteredLogWrites()
        {
            return _logWriters;
        }

        public ILoggerBuilderOptions AddJsonLogger()
        {
            _logWriters.Add(new JsonLogWriter().Build());

            return this;
        }

        public ILoggerBuilderOptions AddXmlLogger()
        {
            _logWriters.Add(new XmlLogWriter().Build());

            return this;
        }

        public ILoggerBuilderOptions AddConsoleLogger()
        {
            _logWriters.Add(new ConsoleLogWriter().Build());

            return this;
        }

        public ILoggerBuilderOptions AddCustomLogger<T>() where T : ILogWriter
        {
            _logWriters.Add(Activator.CreateInstance<T>().Build());

            return this;
        }
    }
}
