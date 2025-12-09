using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models.Interfaces
{
    public interface ILoggerBuilderOptions
    {
        ILoggerBuilderOptions AddJsonLogger(Action<ILogWriterOptions> writerOptions);
        ILoggerBuilderOptions AddJsonLogger();
        ILoggerBuilderOptions AddXmlLogger(Action<ILogWriterOptions> writerOptions);
        ILoggerBuilderOptions AddXmlLogger();
        ILoggerBuilderOptions AddConsoleLogger(Action<ILogWriterOptions> writerOptions);
        ILoggerBuilderOptions AddConsoleLogger();
        ILoggerBuilderOptions AddCustomLogger<T>(Action<ILogWriterOptions> writerOptions) where T : ILogWriter;
        ILoggerBuilderOptions AddCustomLogger<T>() where T : ILogWriter;
        IEnumerable<ILogWriter> GetRegisteredLogWrites();
    }
}
