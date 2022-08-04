using System;
using WebRouterApp.Core.CopyEngineParts;

namespace WebRouterApp.Core.Infrastructure.ApiCallContextParts
{
    public interface IApiCallContextReader
    {
        public Guid CurrentUserId { get; }
        public CopySession? Session { get; }
    }
}