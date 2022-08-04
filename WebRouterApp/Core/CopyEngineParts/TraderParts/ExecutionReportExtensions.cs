using Shared.Models;
using SharedBinance.Models;

namespace WebRouterApp.Core.CopyEngineParts.TraderParts
{
    internal static class ExecutionReportExtensions
    {
        public static Order ToOrder(this ExecutionReport report)
            => report.SelfOrder;
    }
}
