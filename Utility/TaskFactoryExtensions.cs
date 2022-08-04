using System;
using System.Threading;
using System.Threading.Tasks;

namespace Utility
{
    public static class TaskFactoryExtensions
    {
        public static Task StartLongRunning(
            this TaskFactory taskFactory,
            Func<Task> longRunningActionAsync,
            CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(
                longRunningActionAsync,
                stoppingToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            ).Unwrap();
        }

        public static Task StartLongRunning(
            this TaskFactory taskFactory,
            Action longRunningActionAsync,
            CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(
                longRunningActionAsync,
                stoppingToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );
        }
    }
}
