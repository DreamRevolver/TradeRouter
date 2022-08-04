using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Shared.Interfaces;
using SharedBinance.Interfaces;

namespace Utility.ExecutionContext
{
	public sealed class Worker : IExecutionContext
	{
		private readonly ILogger _log;
		
		private readonly AsyncLock _workerLocker = new AsyncLock();
		private SequentialWorker _worker;
		
		public Worker(ILogger log)
		{
			_log = log;
		}
		
		public void Start()
		{
			if (_worker != null)
				return;
			
			using (_workerLocker.Lock())
			{
				if (_worker != null)
					return;

				_worker = new SequentialWorker(_log);
				_worker.Start();
			}
		}

		public async Task Stop()
		{
			if (_worker == null)
				return;
			
			using (_workerLocker.Lock())
			{
				if (_worker == null)
					return;

				var worker = _worker;
				_worker = null;
				
				await worker.Stop();
			}
		}

		public bool Put(Func<object, Task> workItem, object arg = null)
		{
			using (_workerLocker.Lock())
			{
				if (_worker == null)
					return false;
			
				return _worker.Enqueue(() => workItem(arg));
			}
		}

		private sealed class SequentialWorker
		{
			private readonly ILogger _log;

			private readonly CancellationTokenSource _cts = new CancellationTokenSource();
			private readonly Channel<Func<Task>> _workItemChannel = Channel.CreateUnbounded<Func<Task>>();
			private Task _taskExecutionLoop;
		
			public SequentialWorker(ILogger log)
			{
				_log = log;
			}
		
			public void Start()
			{
				_taskExecutionLoop = Task.Factory
					.StartNew(
						function: () => ExecutionLoop(_cts.Token),
						cancellationToken: _cts.Token,
						creationOptions: TaskCreationOptions.LongRunning,
						scheduler: TaskScheduler.Default)
					.Unwrap();
			}

			public async Task Stop()
			{
				try
				{
					// Stop ReSharper complaining about a captured variable being disposed...
					var cts = _cts;
					
					Enqueue(() => 
					{ 
						cts.Cancel();
						return Task.CompletedTask;
					});
					
					_workItemChannel.Writer.Complete();
					await _taskExecutionLoop;
				}
				finally
				{
					_cts.Dispose();
					_taskExecutionLoop = null;
				}
			}

			public bool Enqueue(Func<Task> workItem)
			{
				return _workItemChannel.Writer.TryWrite(workItem);
			}

			private async Task ExecutionLoop(CancellationToken cancellationToken)
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					try
					{
						var workItem = await _workItemChannel.Reader.ReadAsync(cancellationToken);
						await workItem();
					}
					catch (Exception ex)
					{
						_log.Log(
							LogPriority.Error,
							exception: ex,
							source: nameof(ExecutionLoop),
							"Encountered an exception.");
					}
				}
			}
		}
	}
}
