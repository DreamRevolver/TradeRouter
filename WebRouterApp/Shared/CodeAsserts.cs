using System;
using System.Diagnostics.CodeAnalysis;

namespace WebRouterApp.Shared
{
    public static class CodeAsserts
    {
        public static void NotNull([NotNull] object? value, string? message = null)
        {
            if (value != null)
                return;

            throw new InvalidOperationException(message ?? "");
        }
    }
}