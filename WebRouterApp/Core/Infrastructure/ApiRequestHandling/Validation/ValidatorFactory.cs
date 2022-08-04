using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.Validation
{
    public sealed class ValidatorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IValidator<T>? ValidatorFor<T>()
        {
            return _serviceProvider.GetService<IValidator<T>>();
        }
    }
}