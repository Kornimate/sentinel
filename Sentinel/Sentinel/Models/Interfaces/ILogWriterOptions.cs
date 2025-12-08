using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models.Interfaces
{
    public interface ILogWriterOptions
    {
        ILogWriterOptions WithLogFilePath(string path); 
        ILogWriterOptions WithLogFileName(string fileName); 
        ILogWriterOptions WithLoggedClassFilter(string filter); 
        ILogWriterOptions WithMinimumLogLevel(LogLevel logLevel); 
        ILogWriterOptions WithSinkRollTiming(SinkRoll sinkRoll); 
        ILogWriter GetWriterInstance();
    }
}
