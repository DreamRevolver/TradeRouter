using Shared.Interfaces;
using Shared.Models;

using SharedBinance.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using SharedBinance.Interfaces;

using TelegramBotAPI;
namespace AccountTracker
{

	public class AccountTrack
	{

		private List<Order> _masterNewFilledOrders;
		private List<Order> _masterCanceledOrders;
		private readonly FrontEndClient masterConnector;
		private readonly ILogger _logger;
		public readonly IExecutionContext _queue;
		private bool _isRunning;
		private readonly Bot _telegramBot;
		private readonly long chatId;
		public AccountTrack(IExecutionContext queue, ILogger logger, FrontEndClient master)
		{
			_queue = queue;
			_logger = logger;
			masterConnector = master;
			_telegramBot = Bot.GetBot(ConfigurationManager.AppSettings["TeleBotToken"]);
			chatId = long.Parse(ConfigurationManager.AppSettings["TelegramId"]);
		}
		public void Start()
			=> _queue.Put(
				async action =>
				{
					if (_isRunning)
					{
						_logger.Log(LogPriority.AccountTracker, $"Tracking of non master orders already started", "Tracker");

						return;
					}

					try
					{
						_masterNewFilledOrders = masterConnector.MasterOrders;
						_masterCanceledOrders = new List<Order>();
						masterConnector.OrderUpdateEvent += UpdateMasterOrdersFromEvent;
						_isRunning = true;
						_logger.Log(LogPriority.AccountTracker, $"Tracking of non master orders started", "Tracker");
					} catch (Exception exc)
					{
						_logger.Log(LogPriority.Error, exc, "Tracker", "Exception in AccountTrack.Start");
						_logger.Log(LogPriority.Debug, exc, "Tracker", "Exception in AccountTrack.Start");
					}

					await Task.Yield();
				}
			);

		public void Stop()
			=> _queue.Put(
				async action =>
				{
					if (!_isRunning)
					{
						_logger.Log(LogPriority.AccountTracker, $"Tracking of non master orders already stoped", "Tracker");

						return;
					}

					masterConnector.OrderUpdateEvent -= UpdateMasterOrdersFromEvent;
					_isRunning = false;
					_logger.Log(LogPriority.AccountTracker, $"Tracking of non master orders stoped", "Tracker");
					await Task.Yield();
				}
			);

		public void UpdateMasterOrdersFromEvent(ExecutionReport masterReport)
			=> _queue.Put(
				async action =>
				{
					var order = new Order
					{
						ClientId = masterReport.ClientId,
					};

					switch (masterReport.OrderStatus)
					{
					case OrderStatus.NEW:
					case OrderStatus.FILLED:
						if (_masterNewFilledOrders != null)
						{
							if (!_masterNewFilledOrders.Contains(order, Order.CompareClIdAndExchId))
							{
								_masterNewFilledOrders.Add(order);
							}
						}

						break;
					case OrderStatus.CANCELED:
						if (!_masterCanceledOrders.Contains(order, Order.CompareClIdAndExchId))
						{
							_masterCanceledOrders.Add(order);
						}

						break;
					}

					await Task.Yield();
				}
			);

		public void Check(ExecutionReport slaveReport, string slaveName)
			=> _queue.Put(
				async action =>
				{
					if (_isRunning)
					{
						var order = new Order
						{
							ClientId = slaveReport.ClientId,
						};

						switch (slaveReport.OrderStatus)
						{
						case OrderStatus.NEW:
							if (_masterNewFilledOrders != null)
							{
								if (!_masterNewFilledOrders.Contains(order, Order.CompareClIdAndExchId) && !order.ClientId.Contains("snh_"))
								{
									await _telegramBot.SendMessage(
										chatId,
										$"```\nSlave name: {slaveName}\n"
									  + $"Event: Order created manually\n"
									  + $"Side: {slaveReport.OrderSide}\n"
									  + $"Symbol: {slaveReport.Symbol}\n"
										//+ $"Price: {slaveReport.Price}\n"
									  + $"Quantity: {slaveReport.Amount}\n"
									  + $"Order ID: {slaveReport.ExchangeId}\n"
									  + $"Client order ID: {slaveReport.ClientId}```"
									);

									_logger.Log(
										LogPriority.AccountTracker, $"NEW NON MASTER ORDER DETECTED IN {slaveName} <--Report {slaveReport.OrderSide}, {slaveReport.Symbol}, type: {slaveReport.OrderType}, price: {slaveReport.Price}, quantity: {slaveReport.Amount}, order ID: {slaveReport.ExchangeId}, client order ID: {slaveReport.ClientId}, current status: {slaveReport.OrderStatus}," +
										$" StopPrice: {slaveReport.StopPrice}, positionSide: {slaveReport.PositionSide}", "Tracker"
									);
								}
							}

							break;
						case OrderStatus.CANCELED:
							if (!_masterCanceledOrders.Contains(order, Order.CompareClIdAndExchId))
							{
								await _telegramBot.SendMessage(
									chatId,
									$"```\nSlave name: {slaveName}\n"
								  + $"Event: Order canceled manually\n"
								  + $"Side: {slaveReport.OrderSide}\n"
								  + $"Symbol: {slaveReport.Symbol}\n"
									//+ $"Price: {slaveReport.Price}\n"
								  + $"Quantity: {slaveReport.Amount}\n"
								  + $"Order ID: {slaveReport.ExchangeId}\n"
								  + $"Client order ID: {slaveReport.ClientId}```"
								);

								_logger.Log(
									LogPriority.AccountTracker, $"CANCEL MASTER ORDER WICH PRESENT ON MASTER IN {slaveName} <--Report {slaveReport.OrderSide}, {slaveReport.Symbol}, type: {slaveReport.OrderType}, price: {slaveReport.Price}, quantity: {slaveReport.Amount}, order ID: {slaveReport.ExchangeId}, client order ID: {slaveReport.ClientId}, current status: {slaveReport.OrderStatus}," +
									$" StopPrice: {slaveReport.StopPrice}, positionSide: {slaveReport.PositionSide}", "Tracker"
								);
							}

							break;
						}
					}

					await Task.Yield();
				}
			);
		public void HandleBalanceChange(string eventName, List<Balance> balances, string name)
			=> _queue.Put(
				async action =>
				{	
					if (eventName == "WITHDRAW")
					{
						await _telegramBot.SendMessage(chatId, $"{name} balance withdraw\n" + string.Join("\n", balances.Select(i => $"{i.Currency}:"
						  + $" {i.Value}."
						  + $" Balance change: {i.BalanceChange}")));
					}
					else if (eventName == "DEPOSIT")
					{
						await _telegramBot.SendMessage(chatId, $"{name} balance deposit\n" + string.Join("\n", balances.Select(i => $"{i.Currency}:"
						  + $" {i.Value}."
						  + $" Balance change: {i.BalanceChange}")));
					}
					
				}
			);
	}

}
