using System;
using System.Threading;

namespace WebRouterApp.Shared
{
    public static class ReaderWriterLockSlimExtensions
    {
        public static IDisposable BeginReadLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterReadLock();
            return new Guard(() => @lock.ExitReadLock());
        }

        public static IDisposable BeginUpgradeableReadLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterUpgradeableReadLock();
            return new Guard(() => @lock.ExitUpgradeableReadLock());
        }

        public static IDisposable BeginWriteLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterWriteLock();
            return new Guard(() => @lock.ExitWriteLock());
        }

        private class Guard : IDisposable
        {
            private readonly Action _dispose;

            public Guard(Action dispose) => _dispose = dispose;
            public void Dispose() => _dispose();
        }
    }
}
