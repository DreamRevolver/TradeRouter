using System.Runtime.Serialization;

using SharedBinance.Models;

namespace Shared.Models
{

	public sealed class TradingSignal
	{

		public uint leverage { get; set; }
		public bool CloseAll { get; set; } = false;
		public bool ReduceOnly { get; set; }
		public bool ClosePosition { get; set; }
		public PositionSide PositionSide { get; set; }
		public OrderSide OrderSide { get; set; }
		public OrderTimeInForce TimeInForce { get; set; }
		public OrderStatus OrderStatus { get; set; }
		public OrderType OrderType { get; set; }
		public OrderType OriginalOrderType  { get; set; }
		public OrderWorkingType WorkingType { get; set; }
		public string Symbol  { get; set; }
		public string ClientId { get; set; }
		public string ExchangeId { get; set; }
		public double Amount { get; set; }
		public double Price  { get; set; }
		public double StopPrice  { get; set; }
		public double ActivatePrice { get; set; }
		public double PriceRate { get; set; }

		public static explicit operator TradingSignal(Order order)
			=> new TradingSignal()
			{
				ReduceOnly = order.ReduceOnly,
				ClosePosition = order.ClosePosition,
				PositionSide = order.PositionSide,
				OrderSide = order.OrderSide,
				TimeInForce = order.TimeInForce,
				OrderStatus = order.OrderStatus,
				OrderType = order.OrderType,
				OriginalOrderType = order.OriginalOrderType,
				WorkingType = order.WorkingType,
				Symbol = order.Symbol,
				ClientId = order.ClientId,
				ExchangeId = order.ExchangeId,
				Amount = order.Amount,
				Price = order.Price,
				StopPrice = order.StopPrice,
				ActivatePrice = order.ActivatePrice,
				PriceRate = order.PriceRate
			};
		public static explicit operator TradingSignal(ExecutionReport report)
			=> new TradingSignal()
			{
				ReduceOnly = report.ReduceOnly,
				ClosePosition = report.ClosePosition,
				PositionSide = report.PositionSide,
				OrderSide = report.OrderSide,
				TimeInForce = report.TimeInForce,
				OrderStatus = report.OrderStatus,
				OrderType = report.OrderType,
				OriginalOrderType = report.OriginalOrderType,
				WorkingType = report.WorkingType,
				Symbol = report.Symbol,
				ClientId = report.ClientId,
				ExchangeId = report.ExchangeId,
				Amount = report.Amount,
				Price = report.Price,
				StopPrice = report.StopPrice,
				ActivatePrice = report.ActivatePrice,
				PriceRate = report.PriceRate
			};

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

	}

}
