using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models;
using SharedBinance.Models;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Core.CopyEngineParts.TraderParts
{
    public class OrderSet
    {
        public enum ApplyResultCode
        {
            ReportIgnored,
            OrderAdded,
            OrderRemoved,
        }

        private static readonly OrderEqualityComparer OrderEqComparer = new();
        private readonly HashSet<Order> _openOrders = new HashSet<Order>(OrderEqComparer);
        
        private OrderSet()
        {
        }

        public IReadOnlyList<Order> Values => _openOrders.ToList();

        public static OrderSet Empty { get; } = new OrderSet();

        public static OrderSet With(IEnumerable<Order> openOrders)
        {
            var orderSet = new OrderSet();
            orderSet._openOrders.AddRange(openOrders);
            return orderSet;
        }

        public ApplyResultCode Apply(ExecutionReport report)
            => Apply(report.ToOrder());
        
        public ApplyResultCode Apply(Order order)
        {
            switch (order.OrderStatus)
            {
                case OrderStatus.NEW:
                    if (_openOrders.Contains(order))
                        return ApplyResultCode.ReportIgnored;
                    
                    _openOrders.Add(order);
                    return ApplyResultCode.OrderAdded;

                case OrderStatus.CANCELED or OrderStatus.FILLED:
                    _openOrders.Remove(order);
                    return ApplyResultCode.OrderRemoved;
            }

            throw new InvalidOperationException($"Encountered an unsupported order state '{order.OrderStatus}'");
        }

        private class OrderEqualityComparer : IEqualityComparer<Order>
        {
            public bool Equals(Order? left, Order? right)
            {
                if (ReferenceEquals(left, right)) return true;
                if (ReferenceEquals(left, null)) return false;
                if (ReferenceEquals(right, null)) return false;
                if (left.GetType() != right.GetType()) return false;
                return left.ClientId == right.ClientId;
            }

            public int GetHashCode(Order @object) 
                => @object.ClientId?.GetHashCode() ?? 0;
        }
    }
}
