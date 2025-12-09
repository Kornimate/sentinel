using Sentinel.Models;
using Sentinel.Models.LogTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.LogWriters.Interfaces
{
    public interface ILogWriter
    {
        ILogWriter Build();
        void ShutDown();
        void AddLogMessage(object? sender, ILogEntry log);
        void SetFilePath(string filePath);
        void SetFileName(string fileName);
        void SetSubDirectory(string dirName);
        void SetFilter(string filter);
        void SetMinimiumLogLevel(LogLevel logLevel);
        void SetSinkTiming(SinkRoll sinkRoll);
        string? GetFilePath();
        string? GetFileName();
        string? GetSubDirectory();
        bool WriteToConsole();
        Task StartNewBackgroundTask();
        Task? GetBackgroundConsumerTask();
    }
}
