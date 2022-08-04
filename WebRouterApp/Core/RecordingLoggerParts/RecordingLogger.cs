using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.Interfaces;
using SharedBinance.Interfaces;

namespace WebRouterApp.Core.RecordingLoggerParts
{
	public class RecordingLogger : ILogger
	{
		private readonly ILogger _logger;
		private readonly IExecutionContext _executionContext;

		private readonly List<LogRecord> _buffer = new();
		private readonly List<LogRecord> _storage = new();

		public RecordingLogger(ILogger logger, IExecutionContext executionContext)
		{
			_logger = logger;
			_executionContext = executionContext;
			_executionContext.Start();
		}

		private long LastTime => _buffer.LastOrDefault()?.Ticks ?? _storage.LastOrDefault()?.Ticks ?? -1;

		private bool _isInNonDescendingOrder;
		private Task SaveLog(object @object)
		{
			if (@object is not LogRecord logRecord)
			{
				throw new ArgumentException($"expected type: LogRecord received type: {@object.GetType().Name}");
			}
			
			_isInNonDescendingOrder = _isInNonDescendingOrder && LastTime <= logRecord.Ticks;
			_buffer.Add(logRecord);
			
			return Task.CompletedTask;
		}

		private int BoundBinarySearch(long? time, bool isLowerBound)
		{
			if (time is not {} _time)
			{
				return isLowerBound ? 0 : _storage.Count - 1;
			}
			int a = 0, b = _storage.Count;
			while (a + 1 < b)
			{
				var c = (a + b) / 2;
				if (isLowerBound ? _storage[c].Ticks <= _time : _storage[c].Ticks < time)
				{
					b = c;
				} else
				{
					a = c;
				}
			}
			return a;
		}

		private IEnumerable<LogRecord> StaticLoad(long? from, long? to)
		{
			if (!_storage.Any() || (@from ?? long.MinValue) > (to ?? long.MaxValue)
			 || to is {} _to && _storage.First().Ticks > _to
			 || @from is {} _from && _storage.Last().Ticks < _from)
			{
				return Enumerable.Empty<LogRecord>();
			}
			var fromNumber = BoundBinarySearch(from, true);
			var toNumber = BoundBinarySearch(to, false);
			return Enumerable.Range(fromNumber, toNumber - fromNumber + 1).Select(i => _storage[i]);
		}

		private Task FlushBuffer(object _)
		{
			_storage.AddRange(_buffer);
			if (!_isInNonDescendingOrder)
			{
				_storage.Sort();
				_isInNonDescendingOrder = true;
			}
			_buffer.Clear();
			return Task.CompletedTask;
		}

		public Task<IEnumerable<LogRecord>> LoadAsync(long? from, long? to)
		{
			if (!_storage.Any() && !_buffer.Any())
			{
				return Task.FromResult(Enumerable.Empty<LogRecord>());
			}
			if (!_storage.Any() || to is not { } __to || LastTime <= __to)
			{
				_executionContext.Put(FlushBuffer, null);
			}
			var source = new TaskCompletionSource<IEnumerable<LogRecord>>();
			_executionContext.Put(obj =>
			{
				var (_from, _to, _source) = ((long?, long?, TaskCompletionSource<IEnumerable<LogRecord>>))obj;
				_source.SetResult(StaticLoad(_from, _to));
				return Task.CompletedTask;
			}, (from, to, source));
			return source.Task;
		}

		private long _last;
		public void Log(LogPriority priority, string message, string source)
		{
			var spin = new SpinWait();
			var time = DateTime.UtcNow.Ticks;
			while (Interlocked.Exchange(ref _last, time) == _last)
			{
				spin.SpinOnce();
				time = DateTime.UtcNow.Ticks;
			}
			_executionContext.Put(SaveLog, new LogRecord(priority.ToString(), message, source, time));
			_logger.Log(priority, message, source);
		}

		public void Log(LogPriority priority, Exception exception, string source, string description)
			=> Log(priority, source, description);
	}
}
