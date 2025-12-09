using Sentinel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.Logger.Interfaces
{
    public interface ISentinelLogger<T> where T : class
    {
        void LogVerbose(string message, Exception? exception = null);
        void LogDebug(string message, Exception? exception = null);
        void LogInformation(string message, Exception? exception = null);
        void LogWarning(string message, Exception? exception = null);
        void LogError(string message, Exception? exception = null);
        void LogFatal(string message, Exception? exception = null);
        void Log(LogLevel logLevel, string message, Exception? exception = null);

        void ShutDown();
    }
}
