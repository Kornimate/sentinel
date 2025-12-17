using Sentinel.Services.LogWriters;
using Sentinel.Services.LogWriters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models.Options.Interfaces
{
    public interface ILoggerBuilderOptions
    {
        ILoggerBuilderOptions AddJsonLogger(Action<IFileLogWriterOptions> writerOptions);
        ILoggerBuilderOptions AddJsonLogger();
        ILoggerBuilderOptions AddXmlLogger(Action<IFileLogWriterOptions> writerOptions);
        ILoggerBuilderOptions AddXmlLogger();
        ILoggerBuilderOptions AddConsoleLogger(Action<ILogWriterOptions> writerOptions);
        ILoggerBuilderOptions AddConsoleLogger();
        ILoggerBuilderOptions AddCustomLogger<T>(Action<ILogWriterOptions> writerOptions) where T : ILogWriter;
        ILoggerBuilderOptions AddCustomLogger<T>() where T : ILogWriter;
        ILoggerBuilderOptions AddCustomFileLogger<T>(Action<IFileLogWriterOptions> writerOptions) where T : FileLogWriterBase;
        ILoggerBuilderOptions AddCustomFileLogger<T>() where T : FileLogWriterBase;
        IEnumerable<ILogWriter> GetRegisteredLogWrites();
    }
}
