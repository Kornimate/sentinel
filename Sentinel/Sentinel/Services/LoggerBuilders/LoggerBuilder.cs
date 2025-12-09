using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentinel.Models;
using Sentinel.Models.Options;
using Sentinel.Models.Options.Interfaces;
using Sentinel.Services.Logger;
using Sentinel.Services.Logger.Interfaces;
using Sentinel.Services.LoggerBuilders.Interfaces;
using Sentinel.Services.LoggingContext;
using Sentinel.Services.LoggingContext.Interfaces;
using Sentinel.Services.LogWriters;
using Sentinel.Services.LogWriters.Interfaces;

namespace Sentinel.Services.LoggerBuilders
{
    public sealed class LoggerBuilder : ILoggerBuilder
    {
        private static readonly ILoggerContext _context = LoggerContext.GetInstance();
        private static readonly ILoggerBuilderOptions _loggerBuilderOptions = new LoggerBuilderOptions();

        private LoggerBuilder() { }

        public static ILoggerBuilder CreateLogger(Action<ILoggerBuilderOptions> loggerOptions)
        {
            loggerOptions(_loggerBuilderOptions); // loggers first configured

            foreach (ILogWriter logWriter in _loggerBuilderOptions.GetRegisteredLogWrites())
            {
                _context.AddLogWriter(logWriter); // loggers added to context
            }

            return new LoggerBuilder();
        }
        public static ILoggerBuilder CreateLogger()
        {
            _context.AddLogWriter(new ConsoleLogWriter().Build()); // by default add just a console logger

            return new LoggerBuilder();
        }

        public ISentinelLogger<T> GetLogger<T>() where T : class
        {
            return new SentinelLogger<T>();
        }

    }
}
