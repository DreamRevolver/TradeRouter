using System;
using Microsoft.Extensions.DependencyInjection;

namespace WebRouterApp.Shared.Services
{
    public class Factory<T>
    {
        private readonly IServiceProvider _serviceProvider;
        public Factory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T? Create() => _serviceProvider.GetService<T>();
    }
}
