namespace WebRouterApp.Features.Trading.Application.MessageParts
{
    public sealed class OrderModel
    {
        public string? Symbol { get; init; }
        public string? OrderSide { get; init; }
        public string? OrderState { get; init; }
        public double? Price { get; init; }
        public double? Amount { get; init; }
        public string? ClientId { get; init; }
    }
}
