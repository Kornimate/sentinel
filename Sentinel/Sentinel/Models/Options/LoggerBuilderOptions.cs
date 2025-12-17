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
        public LoggerBuilderOptions()
        {
            _logWriters = [];
        }

        public ILoggerBuilderOptions AddJsonLogger(Action<IFileLogWriterOptions> writerOptions)
        {
            IFileLogWriterOptions logWriterOptions = new FileLogWriterOptions(new JsonLogWriter());

            writerOptions(logWriterOptions);

            _logWriters.Add(logWriterOptions.GetWriterInstance().Build());

            return this;
        }

        public ILoggerBuilderOptions AddXmlLogger(Action<IFileLogWriterOptions> writerOptions)
        {
            IFileLogWriterOptions logWriterOptions = new FileLogWriterOptions(new XmlLogWriter());

            writerOptions(logWriterOptions);

            _logWriters.Add(logWriterOptions.GetWriterInstance().Build());

            return this;
        }
        public ILoggerBuilderOptions AddConsoleLogger(Action<ILogWriterOptions> writerOptions)
        {
            ILogWriterOptions logWriterOptions = new LogWriterOptions(new ConsoleLogWriter());

            writerOptions(logWriterOptions);

            _logWriters.Add(logWriterOptions.GetWriterInstance().Build());

            return this;
        }
        public ILoggerBuilderOptions AddCustomFileLogger<T>(Action<IFileLogWriterOptions> writerOptions) where T : FileLogWriterBase
        {
            IFileLogWriterOptions? logWriterOptions = null;

            try
            {
                logWriterOptions = new FileLogWriterOptions(Activator.CreateInstance<T>());
            }
            catch (Exception)
            {
                throw new ArgumentException("ILogWriter derived class's contructor must be parameterless");
            }

            writerOptions(logWriterOptions);

            _logWriters.Add(logWriterOptions.GetWriterInstance().Build());

            return this;
        }
        
        public ILoggerBuilderOptions AddCustomLogger<T>(Action<ILogWriterOptions> writerOptions) where T : ILogWriter
        {
            ILogWriterOptions? logWriterOptions = null;

            try
            {
                logWriterOptions = new LogWriterOptions(Activator.CreateInstance<T>());
            }
            catch (Exception)
            {
                throw new ArgumentException("ILogWriter derived class's contructor must be parameterless");
            }

            writerOptions(logWriterOptions);

            _logWriters.Add(logWriterOptions.GetWriterInstance().Build());

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
            try
            {
                _logWriters.Add(Activator.CreateInstance<T>().Build());
            }
            catch (Exception)
            {
                throw new ArgumentException("ILogWriter derived class's contructor must be parameterless");
            }

            return this;
        }

        public ILoggerBuilderOptions AddCustomFileLogger<T>() where T : FileLogWriterBase
        {
            try
            {
                _logWriters.Add(Activator.CreateInstance<T>().Build());
            }
            catch (Exception)
            {
                throw new ArgumentException("ILogWriter derived class's contructor must be parameterless");
            }

            return this;
        }
    }
}
