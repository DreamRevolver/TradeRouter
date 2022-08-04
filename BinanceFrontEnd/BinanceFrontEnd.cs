using System;
using System.Collections.Generic;
using System.Linq;

using BinanceFuturesConnector;
using Shared.Attributes;
using Shared.Broker;
using Shared.Interfaces;
using Shared.Models;

using SharedBinance.Models;
using SharedBinance.Services;
namespace BinanceFrontEnd
{
	[RequiredParameter("APIKey")]
	[RequiredParameter("APISecret")]
	[RequiredParameter("Url")]
	[RequiredParameter("Wss")]
	public class BinanceFront : FrontEndClient
	{
		private List<Position> _fullPositionsList = new List<Position>();
		private List<Order> _openOrders = new List<Order>();
		private readonly IBrokerClient _client;
		private readonly ILogger _logger;
		private bool _isRunning;
		private readonly string _name;
		public override bool IsRunning => _isRunning;
		public override event Action<Position> PositionChanged = delegate { };
		public override event Action<ExecutionReport> OrderUpdateEvent = delegate { };
		public override event Action<ConnectionEvent> ConnectionEvent = delegate { };
		public override event Action<TradingSignal> SignalEvent = delegate { };
		public override event Action<(string, int)> LeverageChanged = delegate { };
		private void ToTradingSignal(ExecutionReport report)
		{
			try
			{
				switch (report.OrderStatus)
				{
					case OrderStatus.NEW:
					case OrderStatus.CANCELED:
					case OrderStatus.FILLED:
						var signal = (TradingSignal)report;
						lock (_fullPositionsList)
						{
							signal.leverage = (uint)_fullPositionsList.FirstOrDefault(i => i.symbol == signal.Symbol).leverage;
							var pos = _fullPositionsList.FirstOrDefault(i => i.symbol == signal.Symbol && i.positionSide == signal.PositionSide);
							if ((signal.PositionSide == PositionSide.LONG && signal.OrderSide == OrderSide.SELL || signal.PositionSide == PositionSide.SHORT && signal.OrderSide == OrderSide.BUY) && Math.Round(pos.positionAmt - signal.Amount, pos.precision) == 0)
							{
								signal.CloseAll = true;
							}
						}
						SignalEvent(signal);
						break;
				}
            }
			catch (Exception exc)
			{
				_logger.Log(LogPriority.Error, exc, _name, "Exception in BinanceFrontEnd.ToTradingSignal");
				_logger.Log(LogPriority.Debug, exc, _name, "Exception in BinanceFrontEnd.ToTradingSignal");
			}
		}
		public override event Action<MarketBook, string> MarketDataUpdateEvent = delegate { };
		public BinanceFront(ILogger logger, string name, List<(string key, string value)> settings)
		{
			_name = name;
			_logger = logger;
			foreach (var element in settings)
			{
				Settings.Set(element.key, element.value);
			}
			_client = new BinanceClient(_name, _logger, new BinanceConnectorSettings(Settings));
			_client.MarketDataHandler += (_, symbol, book, depth, updates) => MarketDataUpdateEvent(book, symbol);
			_client.AccountConfigUpdateHandler += param =>
			{
				LeverageChanged(param);
				UpdateLeverage(param);
			};
			_client.BrokerEventHandler += (_, what, details) =>
			{
				var @event = what.ToFrontEndEvent();
				if (@event != null)
				{
					ConnectionEvent(@event.Value);
				}
				if (@event == Shared.Models.ConnectionEvent.Logon)
				{
					try
					{
						lock (_fullPositionsList)
						{
							_fullPositionsList = _client.GetPositions().Result?.ToList();
						}
						lock(_openOrders)
						{
							_openOrders = _client.GetAllOpenOrderds().Result;
						}
					} catch (Exception ex)
					{
						_logger.Log(LogPriority.Error, ex, _name, "Exception in BinanceFrontEnd while log on. Try to restart this connector.");
						_logger.Log(LogPriority.Debug, ex, _name, "Exception in BinanceFrontEnd while log on. Try to restart this connector.");
					}
				}
			};
			_client.ExecutionReportHandler += (_, report) =>
			{
				ToTradingSignal(report);
				OrderUpdateEvent(report);
				UpdateOrdersList(report);
				updatePosition(report);
			};
		}
		public override IBrokerClient Connector => _client;
		public override async void Start()
		{
			if (_isRunning)
			{
				return;
			}
			try
			{
                await _client.StartAsync();
				_isRunning = true;
			}
			catch (Exception ex)
			{
				_logger.Log(LogPriority.Error, ex, _name, "Exception in BinanceFrontEnd.Start");
				_logger.Log(LogPriority.Debug, ex, _name, "Exception in BinanceFrontEnd.Start");
			}
		}
		public override void Stop()
		{
			if (_isRunning)
			{
				_client.Stop();
				_isRunning = false;
			}
		}
		private void UpdateLeverage((string, int) param) 
		{
			lock (_fullPositionsList)
			{
				_fullPositionsList.FindAll(i => i.symbol == param.Item1).ForEach(j => j.leverage = param.Item2);
			}
		}
		private void UpdateOrdersList(ExecutionReport report)
		{
			switch (report.OrderStatus)
			{
				case OrderStatus.NEW:
				case OrderStatus.CANCELED:
				case OrderStatus.FILLED:
					var order = report.SelfOrder;
					lock (_openOrders)
					{
						if (_openOrders.Contains(order, Order.CompareClIdAndExchId) &&
							(order.OrderStatus == OrderStatus.CANCELED || order.OrderStatus == OrderStatus.FILLED))
						{
							_openOrders.RemoveAll(i => i.ClientId == order.ClientId);
						}
						if (!_openOrders.Contains(order, Order.CompareClIdAndExchId) && (order.OrderStatus == OrderStatus.NEW))
						{
							_openOrders.Add(order);
						}
					}
					break;
			}
		}
		private void updatePosition(ExecutionReport report)
		{
			if (report.OrderStatus == OrderStatus.FILLED)
			{
				lock (_fullPositionsList)
				{
					var position = _fullPositionsList.FirstOrDefault(i => i.symbol == report.Symbol && i.positionSide == report.PositionSide);
					if (position != null)
					{
						_fullPositionsList.RemoveAll(i => i.symbol == report.Symbol && i.positionSide == report.PositionSide);
						position.UpdateFromReport(report);
						_fullPositionsList.Add(position);
						PositionChanged(position);
					}
				}
				
			}
		}
		public override List<Order> MasterOrders
		{
			get
			{
				lock (_openOrders)
				{
					var _openOrdersClone = _openOrders.Select(i => (Order)i.Clone()).ToList();
					return _openOrdersClone;
				}
			}
		}
		public override List<Position> MasterPositions
		{
			get
			{
				lock (_fullPositionsList)
				{
					var _positionsClone = _fullPositionsList.Where(i=> i.positionAmt != 0).Select(i => (Position)i.Clone()).ToList();
					return _positionsClone;
				}
			}
		}
	}
}
