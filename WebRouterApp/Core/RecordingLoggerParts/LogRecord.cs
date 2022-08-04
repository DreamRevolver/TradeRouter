using System;

namespace WebRouterApp.Core.RecordingLoggerParts
{
	public sealed record LogRecord(string Priority, string Message, string Source, long Ticks) : IComparable<LogRecord>
	{
		public int CompareTo(LogRecord? other) =>
			other != null
				? Ticks.CompareTo(other.Ticks)
				: throw new ArgumentNullException(nameof(other), "LogRecord must not be null");
	}
}
