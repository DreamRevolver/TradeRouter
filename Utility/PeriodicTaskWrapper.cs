using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Shared.Interfaces;

using SharedBinance.interfaces;

namespace CommonExchange
{

	internal sealed class PeriodicTaskWrapper : IPeriodicTask
	{

		private CancellationToken _token;
		private readonly ILogger _logger;
		private readonly string _name;

		public PeriodicTaskWrapper([NotNull] ILogger logger, [CanBeNull] string name)
		{
			_logger = logger;
			_name = name;
		}

		public bool IsCancellationRequested
			=> _token.IsCancellationRequested;

		[CanBeNull]
		public Task Task { get; private set; }

		public async Task Start([NotNull] PeriodicTaskAction<object, CancellationToken> action, [NotNull] PeriodicTaskParams param, CancellationToken token)
		{
			Debug.Assert(_logger != null, nameof(_logger) + " != null");
			var _action = action;
			if (param.delay != 0)
			{
				await Task.Delay(param.delay, token);
			}
			Task = Task.Run(
				async () =>
				{
					while (!token.IsCancellationRequested)
					{
						try
						{
							await (_action(null, token) ?? throw new Exception("PERIODIC TASK WRAPPER AWAIT NULL ACTION RETURN"));
						} catch (Exception ex)
						{
							_logger.Log(LogPriority.Debug, $"Exception in PeriodicTaskWrapper.Start | Message: {ex.Message} | StackTrace: {ex.StackTrace}", _name);
						} finally
						{
							await Task.Delay(param.period, token);
						}
					}
				}, token
			);
		}

	}

}
