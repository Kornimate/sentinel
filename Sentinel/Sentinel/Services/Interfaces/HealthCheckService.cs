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
        private CancellationTokenSource _cts;
        private PeriodicTimer _timer;
        public HealthCheckService()
        {
            _cts = new CancellationTokenSource();
            _tasksToCheck = new Dictionary<ILogWriter, Task>();
            _ = RunAsync(_cts.Token);
        }
        private async Task RunAsync(CancellationToken ct)
        {
            _timer = new(TimeSpan.FromSeconds(5));

            try
            {
                while (await _timer.WaitForNextTickAsync(ct))
                {
                    CheckListOfTasksIfAlive();
                }
            }
            catch (Exception) { } // just to exit the timer loop
        }

        private void CheckListOfTasksIfAlive()
        {
            foreach (var (logWriter, task) in _tasksToCheck)
            {
                if (task.IsCompleted) // restart task if needed
                {
                    task.Dispose();
                    Task newTask = logWriter.StartNewBackgroundTask();
                    _tasksToCheck[logWriter] = newTask;
                }
            }
        }

        public void AddTaskToCheck(ILogWriter taskOwner, Task taskToCheck)
        {
            _tasksToCheck.Add(taskOwner, taskToCheck);
        }

        public void ShutDown()
        {
            _cts?.Cancel();
            _timer?.Dispose();
        }
    }
}
