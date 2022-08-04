using System;

namespace SharedBinance.interfaces
{
	public interface IActionScheduler
	{
		IDisposable Schedule(Action action);
	}
}
