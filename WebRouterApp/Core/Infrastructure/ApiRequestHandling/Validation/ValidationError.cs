using System;
using System.Collections.Generic;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts;

namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.Validation
{
    public sealed class ValidationError : IApiError
    {
        public ValidationError(IEnumerable<PropertyValidationError> properties)
        {
            Properties = properties;
        }

        public string Tag => ErrorTags.ValidationError;
        public IEnumerable<PropertyValidationError> Properties { get; }
    }
    

    public sealed class PropertyValidationError
    {
        private readonly List<string> _errors;
            
        public PropertyValidationError(string propertyName, string attemptedValue)
        {
            _errors = new List<string>();
                
            PropertyName = propertyName;
            AttemptedValue = attemptedValue;
        }

        public string PropertyName { get; }
        public string AttemptedValue { get; }
        public IEnumerable<string> Errors => _errors;

        public void AddError(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentException(
                    message: $"{nameof(error)} is null or whitespace", 
                    paramName: nameof(error));
            }

            _errors.Add(error);
        }
    }
}