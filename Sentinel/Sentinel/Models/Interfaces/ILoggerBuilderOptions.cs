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
        ILoggerBuilderOptions AddXmlLogger(Action<ILogWriterOptions> writerOptions);
        ILoggerBuilderOptions AddConsoleLogger(Action<ILogWriterOptions> writerOptions);
        ILoggerBuilderOptions AddCustomLogger(Action<ILogWriterOptions> writerOptions);
        IEnumerable<ILogWriter> GetRegisteredLogWrites();
    }
}
