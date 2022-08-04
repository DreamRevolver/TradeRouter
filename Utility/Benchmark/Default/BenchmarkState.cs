using System;
namespace Utility.Benchmark.Default
{

	public struct BenchmarkState
	{

		public int iterationCount;
		public long totalTime;
		public long totalElapsed;
		public long minElapsed;
		public long maxElapsed;
		public long totalDelay;
		public long minDelay;
		public long maxDelay;

		private static readonly BenchmarkState INIT = new()
		{
			iterationCount = 0,
			totalTime = 0,
			totalElapsed = 0,
			minElapsed = long.MaxValue,
			maxElapsed = long.MinValue,
			totalDelay = 0,
			minDelay = long.MaxValue,
			maxDelay = long.MinValue
		};

		private long _initTime;
		internal static BenchmarkState Initialize()
		{
			var result = INIT;
			result._initTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			return result;
		}

		public void Reset()
			=> this = Initialize();

		internal void Apply(BenchmarkIteration data)
		{
			++iterationCount;
			totalTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _initTime;
			totalElapsed += data.elapsed;
			minElapsed = Math.Min(minElapsed, data.elapsed);
			maxElapsed = Math.Max(maxElapsed, data.elapsed);
			totalDelay += data.delay;
			minDelay = Math.Min(minDelay, data.delay);
			maxDelay = Math.Max(maxDelay, data.delay);
		}

		public override string ToString() => $"ITER: {iterationCount}|TIME: {totalTime}ms|\n"
		  + $"ELAPSED => |TOTAL: {totalElapsed}ms|MIN: {minElapsed}ms|MAX: {maxElapsed}ms|AVG: {totalElapsed / (double)iterationCount}ms|\n"
		  + $"DELAY => |TOTAL: {totalDelay}ms|MIN: {minDelay}ms|MAX: {maxDelay}ms|AVG: {totalDelay / (double)iterationCount}ms|";

	}

}
