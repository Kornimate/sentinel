using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentinel.Services.LogWriters.Interfaces;

namespace Sentinel.Services.HelathChecks.Interfaces
{
    internal interface IHealthCheckService
    {
        void AddTaskToCheck(ILogWriter taskOwner, Task taskToCheck);
        void ShutDown();
    }
}
