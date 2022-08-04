using System;
using System.Runtime.Serialization;

using SharedBinance.Models;

namespace Shared.Models
{
    public class ExecutionReport : ICloneable
    {

        public bool ReduceOnly { get; set; }
        public bool ClosePosition { get; set; }
        public PositionSide PositionSide  { get; set; }
        public OrderSide OrderSide { get; set; }
        public OrderTimeInForce TimeInForce { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderType OrderType { get; set; }
        public OrderType OriginalOrderType  { get; set; }
        public OrderWorkingType WorkingType { get; set; }
        public string Symbol  { get; set; }
        public string ClientId  { get; set; }
        public string ExchangeId  { get; set; }
        public double Amount  { get; set; }
        public double Price  { get; set; }
        public double StopPrice  { get; set; }
        public double ActivatePrice { get; set; }
        public double PriceRate { get; set; }
        public double LastFilledPrice { get; set; }

        public double FilledQty { get; set; }

        public double CumQty { get; set; }

        public double LeavesQty { get; set; }

        public double AvgPx { get; set; }

        public long LastUpdate { get; set; }

        public UpdateEventState UpdateState { get; set; }
        [IgnoreDataMember]
        public Order SelfOrder => new Order()
            {
                ReduceOnly = ReduceOnly,
                ClosePosition = ClosePosition,
                PositionSide = PositionSide,
                OrderSide = OrderSide,
                TimeInForce = TimeInForce,
                OrderStatus = OrderStatus,
                OrderType = OrderType,
                OriginalOrderType = OriginalOrderType,
                WorkingType = WorkingType,
                Symbol = Symbol,
                ClientId = ClientId,
                ExchangeId = ExchangeId,
                Amount = Amount,
                Price = Price,
                StopPrice = StopPrice,
                ActivatePrice = ActivatePrice,
                PriceRate = PriceRate
            };

        public object Clone()
            => new ExecutionReport()
            {
                ReduceOnly = ReduceOnly,
                ClosePosition = ClosePosition,
                PositionSide = PositionSide,
                OrderSide = OrderSide,
                TimeInForce = TimeInForce,
                OrderStatus = OrderStatus,
                OrderType = OrderType,
                OriginalOrderType = OriginalOrderType,
                WorkingType = WorkingType,
                Symbol = Symbol,
                ClientId = ClientId,
                ExchangeId = ExchangeId,
                Amount = Amount,
                Price = Price,
                StopPrice = StopPrice,
                ActivatePrice = ActivatePrice,
                PriceRate = PriceRate,
                LastFilledPrice = LastFilledPrice,
                FilledQty = FilledQty,
                CumQty = CumQty,
                LeavesQty = LeavesQty,
                AvgPx = AvgPx,
                LastUpdate = LastUpdate,
                UpdateState = UpdateState
            };

    }
}
