using System;
using System.Collections.Concurrent;
using Nito.AsyncEx;

namespace WebRouterApp.Features.Subscribers.Application.Services
{
    public sealed class TraderLock
    {
        private readonly ConcurrentDictionary<Guid, AsyncLock> _lockMap = new();

        public AsyncLock For(Guid traderId)
        {
            var traderLock = _lockMap.GetOrAdd(traderId, _ => new AsyncLock());
            return traderLock;
        }
    }
}
