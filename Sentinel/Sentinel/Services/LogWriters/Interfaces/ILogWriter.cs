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
        void SetFilter(string filter);
        void SetMinimiumLogLevel(LogLevel logLevel);
        void SetLogContainerSize(int size);
        bool WriteToConsole();
        Task StartNewBackgroundTask();
        Task? GetBackgroundConsumerTask();
    }
}
