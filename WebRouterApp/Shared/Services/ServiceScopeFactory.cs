using System;
using Microsoft.Extensions.DependencyInjection;

namespace WebRouterApp.Shared.Services
{
    public class ServiceScopeFactory<T>
        where T : notnull
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ServiceScopeFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Scoped<T> BeginRequired()
        {
            var serviceScope = _serviceScopeFactory.CreateScope();
            var service = serviceScope.ServiceProvider.GetRequiredService<T>();
            return new Scoped<T>(serviceScope, service);
        }
    }

    public class Scoped<T> : IDisposable
    {
        private IServiceScope _serviceScope;
        public Scoped(IServiceScope serviceScope, T value)
        {
            _serviceScope = serviceScope;
            Value = value;
        }

        public T Value { get; }

        public void Dispose()
        {
            _serviceScope.Dispose();
            _serviceScope = null!;
        }
    }
}
