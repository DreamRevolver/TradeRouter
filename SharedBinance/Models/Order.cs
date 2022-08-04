using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Shared.Models;

using SharedBinance.Extensions;
using SharedBinance.Interfaces;

namespace SharedBinance.Models
{
    public class Order: ICloneable, IUpdateEvent
    {

		public OrderExecutionType ExecutionType { get; set; }

		public bool ReduceOnly { get; set; }
		public bool ClosePosition { get; set; }
		public PositionSide PositionSide  { get; set; }
		public OrderSide OrderSide  { get; set; }
		public OrderTimeInForce TimeInForce { get; set; }
		public OrderStatus OrderStatus  { get; set; }
		public OrderType OrderType  { get; set; }
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
		

		[IgnoreDataMember]
        public static IEqualityComparer<Order> CompareClIdAndExchId { get; } = new ClientIdExchIdComparer();
		public long? LastUpdate  { get; set; }

        private class ClientIdExchIdComparer : IEqualityComparer<Order>
        {
            public bool Equals(Order x, Order y) => x.ClientId.Equals(y.ClientId);
            public int GetHashCode(Order obj) => obj.GetHashCode();
        }
        //Comparer for sorting List<Order>
        public class Comparer : IComparer<Order>
        {
            public readonly OrderSortField _sortField;
            public readonly int _sign;
            public Comparer(OrderSortField sortField, bool sign)
            {
                _sortField = sortField;
                _sign = sign ? 1 : -1;
            }
            public int Compare(Order x, Order y)
			{
				if (x is null || y is null)
                {
                    return (x is null && y is null ? 0 : x is null ? -1 : 1) * _sign;
                }

				return _sortField switch
				{
					OrderSortField.Symbol => x.Symbol.NullableCompare(y.Symbol) * _sign,
					OrderSortField.Side => ((int) x.PositionSide).NullableCompare((int) y.PositionSide) * _sign,
					OrderSortField.Status => ((int) x.OrderStatus).NullableCompare((int) y.OrderStatus) * _sign,
					OrderSortField.Id => x.ExchangeId.NullableCompare(y.ExchangeId) * _sign,
					OrderSortField.ClientId => x.ClientId.NullableCompare(y.ClientId) * _sign,
					OrderSortField.Price => x.Price.NullableCompare(y.Price) * _sign,
					OrderSortField.Qty => x.Amount.NullableCompare(y.Amount) * _sign,
					_ => throw new ArgumentOutOfRangeException()
				};
			}
        }

		[IgnoreDataMember]
		public ExecutionReport SelfReport => new ExecutionReport()
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
			=> new Order {
				ExecutionType = ExecutionType,
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
				LastUpdate = LastUpdate
			};

		public int CompareTo(IUpdateEvent other)
			=> string.Compare(Identifier, other.Identifier, StringComparison.Ordinal);

		[IgnoreDataMember]
		public string Identifier => ClientId;
		[IgnoreDataMember]
		public long Time => LastUpdate ?? 0L;
		[IgnoreDataMember]
		private UpdateEventState? _state;

		[IgnoreDataMember]
		public UpdateEventState State { get => _state ?? ((OrderStatus & (OrderStatus.FILLED | OrderStatus.CANCELED)) != 0 ? UpdateEventState.CLOSED : UpdateEventState.OPEN); set => _state = value; }
    }

}
