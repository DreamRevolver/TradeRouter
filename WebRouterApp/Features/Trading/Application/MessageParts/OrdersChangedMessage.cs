using System;
using System.Collections.Generic;
using System.Linq;

namespace WebRouterApp.Features.Trading.Application.MessageParts
{
    public class OrdersChangedMessage : ITradingMessage
    {
        public OrdersChangedMessage(Guid traderId, OrdersChangeSet changeSet)
        {
            TraderId = traderId;
            ChangeSet = changeSet;
        }

        public string Tag => "Trading.OrdersChanged";

        public Guid TraderId { get; }
        public OrdersChangeSet ChangeSet { get; }
    }

    public class OrdersChangeSet
    {
        public OrdersChangeSet(
            IEnumerable<OrderModel> added, 
            IEnumerable<OrderModel> removed)
        {
            Added = added.ToList();
            Removed = removed.ToList();
        }

        public IReadOnlyList<OrderModel> Added { get; }
        public IReadOnlyList<OrderModel> Removed { get; }
    }

    public static class OrdersChangeSetExtensions
    {
        public static bool Any(this OrdersChangeSet changeSet)
            => changeSet.Added.Any() ||
               changeSet.Removed.Any();

        public static bool IsEmpty(this OrdersChangeSet changeSet)
            => !changeSet.Any();
    }
}