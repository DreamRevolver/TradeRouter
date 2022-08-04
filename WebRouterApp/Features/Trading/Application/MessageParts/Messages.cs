using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models;
using SharedBinance.Models;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Features.Trading.Application.MessageParts
{
    public static class Messages
    {
        public static OrdersChangedMessage? OrdersChanged(
            Guid traderId,
            IEnumerable<Order>? added = null,
            IEnumerable<Order>? removed = null)
        {
            var changeSet = new OrdersChangeSet(
                added: added?.ToModels() ?? EmptyReadOnlyList<OrderModel>.Instance,
                removed: removed?.ToModels() ?? EmptyReadOnlyList<OrderModel>.Instance);

            if (changeSet.IsEmpty())
                return null;

            return new OrdersChangedMessage(traderId, changeSet);
        }

        public static OrdersSnapshotMessage OrdersSnapshot(
            Guid traderId,
            IEnumerable<Order> orders)
        {
            return new OrdersSnapshotMessage(traderId, orders.ToModels());
        }

        public static PositionsChangedMessage? PositionsChanged(
            Guid traderId,
            IEnumerable<Position>? added = null,
            IEnumerable<Position>? updated = null,
            IEnumerable<Position>? removed = null)
        {
            var changeSet = new PositionsChangeSet(
                added: added?.ToModels() ?? EmptyReadOnlyList<PositionModel>.Instance,
                updated: updated?.ToModels() ?? EmptyReadOnlyList<PositionModel>.Instance,
                removed: removed?.ToModels() ?? EmptyReadOnlyList<PositionModel>.Instance);
        
            if (changeSet.IsEmpty())
                return null;

            return new PositionsChangedMessage(traderId, changeSet);
        }
        
        public static PositionsSnapshotMessage PositionsSnapshot(
            Guid traderId,
            IEnumerable<Position> positions)
        {
            return new PositionsSnapshotMessage(traderId, positions.ToModels());
        }

        private static IReadOnlyList<OrderModel> ToModels(this IEnumerable<Order> orders) =>
            orders.Select(it => it.ToModel()).ToReadOnlyList();

        private static OrderModel ToModel(this Order order)
        {
            return new OrderModel 
            {
                Symbol = order.Symbol, 
                OrderSide = order.OrderSide.ToString(), 
                OrderState = order.OrderStatus.ToString(), 
                Price = order.Price,
                Amount = order.Amount, 
                ClientId = order.ClientId, 
            };
        }

        private static IReadOnlyList<PositionModel> ToModels(this IEnumerable<Position> positions) =>
            positions.Select(it => it.ToModel()).ToReadOnlyList();

        private static PositionModel ToModel(this Position position)
        {
            return new PositionModel 
            {
                Symbol = position.symbol,
                EntryPrice = position.entryPrice,
                Leverage = position.leverage,
                PositionAmt = position.positionAmt,
                UnRealizedProfit = position.unRealizedProfit,
                PositionSide = position.positionSide.ToString(),
            };
        }
    }
}