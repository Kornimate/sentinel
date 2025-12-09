using Sentinel.Services.Logger.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.LoggerBuilders.Interfaces
{
    public interface ILoggerBuilder
    {
        ISentinelLogger<T> GetLogger<T>() where T : class;
    }
}
