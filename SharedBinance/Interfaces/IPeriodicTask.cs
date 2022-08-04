using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace SharedBinance.interfaces
{
    public interface IPeriodicTask
    {
        [NotNull]
        Task Start(PeriodicTaskAction<object, CancellationToken> action, PeriodicTaskParams param, CancellationToken token);
        bool IsCancellationRequested { get; }
        Task Task { get; }
    }

    public class PeriodicTaskParams
    {
        public int delay;
        public int period;
    }

    public delegate Task PeriodicTaskAction<in T1, in T2>(T1 arg1, T2 arg2);
}
