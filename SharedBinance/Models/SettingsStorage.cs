using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

using Shared.Attributes;
using Shared.Interfaces;
namespace Shared.Models
{
    
    public sealed class SettingsStorage : IDataStorage
    {
        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();
        
        public SettingsStorage([NotNull] IEndpoint client)
            => _parameters = client.GetType()
                .GetCustomAttributes<RequiredParameterAttribute>()
                .Select(it => it.Name)
                .ToDictionary(key => key, value => (string)null);
        
        public string Get(string key)
            => _parameters.TryGetValue(key, out var value) ? value : null;
        
        public bool Set(string key, string value)
        {
            if (!_parameters.ContainsKey(key))
                return false;

            _parameters[key] = value;
            return true;
        }

        public string[] AvailableKeys => _parameters.Keys.ToArray();
    }
}
