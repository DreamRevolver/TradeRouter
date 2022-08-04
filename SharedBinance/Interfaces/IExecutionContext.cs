using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharedBinance.Interfaces
{
	public interface IExecutionContext
	{
		void Start();
		Task Stop();
		bool Put([NotNull] Func<object, Task> action, [CanBeNull] object item = null);
	}
}
