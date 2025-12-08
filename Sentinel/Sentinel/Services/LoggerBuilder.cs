using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentinel.Models;
using Sentinel.Models.Interfaces;
using Sentinel.Services.Interfaces;

namespace Sentinel.Services
{
    public sealed class LoggerBuilder : ILoggerBuilder
    {
        private static readonly ILoggerContext _context = LoggerContext.GetInstance();
        private static readonly ILoggerBuilderOptions _loggerBuilderOptions = new LoggerBuilderOptions();

        private readonly ISentinelLogger _logger;

        private LoggerBuilder()
        {
            _logger = new SentinelLogger();
        }

        public ISentinelLogger Logger { get => _logger; }

        public static ILoggerBuilder CreateLogger(Action<ILoggerBuilderOptions> loggerOptions)
        {
            loggerOptions(_loggerBuilderOptions);

            foreach (ILogWriter logWriter in _loggerBuilderOptions.GetRegisteredLogWrites())
            {
                _context.AddLogWriter(logWriter);
            }

            return new LoggerBuilder();
        }

    }
}
