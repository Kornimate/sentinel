using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.Interfaces
{
    internal interface IHealthCheckService
    {
        void AddTaskToCheck(ILogWriter taskOwner, Task taskToCheck);
        void ShutDown();
    }
}
