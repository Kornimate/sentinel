using Sentinel.Services.LogWriters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models.Options.Interfaces
{
    public interface IFileLogWriterOptions : ILogWriterOptionsBase
    {
        IFileLogWriterOptions WithLogFilePath(string path);
        IFileLogWriterOptions WithLogFileName(string fileName);
        IFileLogWriterOptions WithLogFileSubDirectory(string subDirectory);
        IFileLogWriterOptions WithSinkRollTiming(SinkRoll sinkRoll);
        IFileLogWriterOptions WithLogContainerSize(int size);
        IFileLogWriterOptions WithLoggedClassFilter(string filter);
        IFileLogWriterOptions WithMinimumLogLevel(LogLevel logLevel);
    }
}
