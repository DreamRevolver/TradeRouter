using System;
using System.Threading;

namespace SharedBinance.Models
{
	public sealed class RecurrentAction : IDisposable
	{
		private readonly Timer _timer;
		
		public RecurrentAction(Action action, long intervalMillisecs)
		{
			_timer = new Timer(
				callback: _ => action(), 
				state: null, 
				dueTime: 0, 
				period: intervalMillisecs);
		}

		public void Dispose()
		{
			_timer.Change(Timeout.Infinite, Timeout.Infinite);
			_timer.Dispose();
		}
	}
}
