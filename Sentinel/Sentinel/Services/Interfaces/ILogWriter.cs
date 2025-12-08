using Sentinel.Models;
using Sentinel.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.Interfaces
{
    public interface ILogWriter
    {
        void AddLogMessage(object? sender, ILogEntry log);
        void SetFilePath(string filePath);
        void SetFileName(string fileName);
        void SetSubDirectory(string dirName);
        void SetFilter(string filter);
        void SetMinimiumLogLevel(LogLevel logLevel);
        void SetSinkTiming(SinkRoll sinkRoll);
        Task StartNewBackgroundTask();
        Task? GetBackgroundConsumerTask();
    }
}
