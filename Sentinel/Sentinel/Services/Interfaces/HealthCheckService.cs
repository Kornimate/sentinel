using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.Interfaces
{
    internal class HealthCheckService : IHealthCheckService
    {
        private IDictionary<ILogWriter, Task> _tasksToCheck;
        public HealthCheckService()
        {
            _tasksToCheck = new Dictionary<ILogWriter, Task>();
            
        }
        public async Task RunAsync()
        {
            using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));

            while( await timer.WaitForNextTickAsync())
            {
                CheckListOfTasksIfAlive();
            }
        }

        public void AddTaskToCheck(ILogWriter taskOwner, Task taskToCheck)
        {
            _tasksToCheck.Add(taskOwner, taskToCheck);
        }

        private void CheckListOfTasksIfAlive()
        {
            foreach(var (logWriter, task)  in _tasksToCheck)
            {
                if (task.IsCompleted) // restart task if needed
                {
                    task.Dispose();
                    Task newTask = logWriter.StartNewBackgroundTask();
                    _tasksToCheck[logWriter] = newTask;
                }
            }
        }
    }
}
