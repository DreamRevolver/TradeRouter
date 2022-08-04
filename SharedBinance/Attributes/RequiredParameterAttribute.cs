
using System;
namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class RequiredParameterAttribute : Attribute
    {
        internal string Name { get; }
        public RequiredParameterAttribute(string name)
            => Name = name;
    }
}
