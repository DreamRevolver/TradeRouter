using System;
using System.Collections.Generic;

namespace WebRouterApp.Features.Trading.Application.MessageParts
{
    public class OrdersSnapshotMessage : ITradingMessage
    {
        public OrdersSnapshotMessage(Guid traderId, IReadOnlyList<OrderModel> orders)
        {
            TraderId = traderId;
            Orders = orders;
        }

        public string Tag => "Trading.OrdersSnapshot";

        public Guid TraderId { get; }
        public IReadOnlyList<OrderModel> Orders { get; }
    }
}