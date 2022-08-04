namespace WebRouterApp.Features.Trading.Application.MessageParts
{
    public sealed class PositionModel
    {
        public string? Symbol { get; init; }
        public double? EntryPrice { get; init; }
        public int? Leverage { get; init; }
        public double? PositionAmt { get; init; }
        public double? UnRealizedProfit { get; init; }
        public string? PositionSide { get; init; }
    }
}
