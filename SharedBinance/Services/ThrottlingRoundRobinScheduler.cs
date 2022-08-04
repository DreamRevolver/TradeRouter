using System;
using System.Collections.Concurrent;
using System.Threading;
using SharedBinance.interfaces;
using SharedBinance.Models;

namespace SharedBinance.Services
{
	public class ThrottlingRoundRobinScheduler : IActionScheduler, IDisposable
	{
		private readonly ConcurrentQueue<ScheduledAction> _schedule = new ConcurrentQueue<ScheduledAction>();
		private readonly RecurrentAction _recurrentAction;

		public ThrottlingRoundRobinScheduler(int interval)
		{
			_recurrentAction = new RecurrentAction(InvokeNextScheduled, interval);
		}

		public static ThrottlingRoundRobinScheduler Instance { get; } = new ThrottlingRoundRobinScheduler(5 * 1000);

		public void Dispose() => _recurrentAction.Dispose();

		public IDisposable Schedule(Action action)
		{
			var scheduledAction = new ScheduledAction(action);
			_schedule.Enqueue(scheduledAction);
			
			return scheduledAction;
		}

		private void InvokeNextScheduled()
		{
			while (_schedule.TryDequeue(out var scheduledAction))
			{
				if (scheduledAction.IsCancellationRequested)
				{
					// Continue without rescheduling this action.
					continue;
				}

				scheduledAction.Invoke();
				
				// Reschedule by moving to the back of the queue.
				_schedule.Enqueue(scheduledAction);
				
				break;
			}
		}

		private sealed class ScheduledAction : IDisposable
		{
			private readonly CancellationTokenSource _cts = new CancellationTokenSource();
			
			private readonly Action _action;
			public ScheduledAction(Action action)
			{
				_action = action;
			}

			public bool IsCancellationRequested => _cts.Token.IsCancellationRequested;

			public void Invoke() =>  _action.Invoke();
			public void Dispose() => _cts.Cancel();
		}
	}
}
