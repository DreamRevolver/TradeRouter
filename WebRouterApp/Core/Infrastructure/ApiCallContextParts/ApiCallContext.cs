using System;
using System.Security.Claims;
using WebRouterApp.Core.CopyEngineParts;
using WebRouterApp.Shared;

namespace WebRouterApp.Core.Infrastructure.ApiCallContextParts
{
    public class ApiCallContext : IApiCallContextReader
    {
        private readonly CopyEngine _copyEngine;

        public ApiCallContext(CopyEngine copyEngine)
        {
            _copyEngine = copyEngine;
        }

        public void InitializeWith(ClaimsPrincipal principal)
        {
            CurrentUserId = principal.Id<Guid>();
        }

        public Guid CurrentUserId { get; private set; }
        public CopySession? Session => _copyEngine.GetSession(CurrentUserId);
    }
}