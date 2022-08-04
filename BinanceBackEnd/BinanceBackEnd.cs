using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using BinanceFuturesConnector;
using Shared.Attributes;
using Shared.Broker;
using Shared.Core;
using Shared.Interfaces;
using Shared.Models;

using SharedBinance.Interfaces;
using SharedBinance.Models;
using SharedBinance.Services;

using TelegramBotAPI;

namespace BinanceBackEnd
{

	[RequiredParameter("APIKey")]
	[RequiredParameter("APISecret")]
	[RequiredParameter("Mode")]
	[RequiredParameter("ModeValue")]
	[RequiredParameter("CopyMasterOrders")]
	[RequiredParameter("Url")]
	[RequiredParameter("Wss")]
	[RequiredParameter("RetryAttempts")]
	[RequiredParameter("RetryDelayMs")]
	public class BinanceBack : BackEndClient
	{

		private List<Position> _fullPositionsList = new List<Position>();
		private List<Balance> _balances = new List<Balance>();
		private readonly IBrokerClient _client;
		private readonly FrontEndClient _masterConnector;
		private readonly ILogger _logger;
		private readonly IExecutionContext _queue;
		private readonly string _name;
		private bool _isRunning;
		private IEnumerable<Instrument> _instruments;
		private readonly List<string> _ordersId = new List<string>();
		public override event Action<string, List<Balance>, string> BalanceChangeEvent = delegate { };
		public override event Action<Position> PositionChanged = delegate { };
		public override event Action<ConnectionEvent> ConnectionEvent = delegate { };
		public override event Action<MarketBook, string> MarketDataUpdateEvent = delegate { };
		public override event Action<ExecutionReport, string> OrderUpdateEvent = delegate { };
		public override bool IsRunning => _isRunning;
		public BinanceBack(IExecutionContext queue, ILogger logger, string name, List<(string key, string value)> settings, FrontEndClient master)
		{
			_queue = queue;
			_logger = logger;
			_name = name;
			_masterConnector = master;
			Bot.GetBot(GetTelegramToken(settings));
			foreach (var element in settings)
			{
				Settings.Set(element.key, element.value);
			}
			_client = new BinanceClient(_name, _logger, new BinanceConnectorSettings(Settings));
			_client.BrokerEventHandler += (_, what, details) =>
			{
				var @event = what.ToFrontEndEvent();

				if (@event != null)
				{
					ConnectionEvent(@event.Value);
				}
			};
			_client.MarketDataHandler += (_, symbol, book, depth, updates) => MarketDataUpdateEvent(book, symbol);
			_client.BalanceUpdateHandler += (eventName, balances) =>
			{
				UpdateBalances(balances);
				BalanceChangeEvent(eventName, balances, _name);
			};
			_client.ExecutionReportHandler += (_, report) =>
			{
				OrderUpdateEvent(report, _name);
				updatePosition(report);
			};
			_client.AccountConfigUpdateHandler += param =>
			{
				UpdateLeverage(param);
			};
		}
		
		private static string GetTelegramToken(IReadOnlyCollection<(string key, string value)> settings)
		{
			const string teleBotTokenKey = "TeleBotToken";
			
			var telegramToken = settings.Any(it => it.key == teleBotTokenKey)
				? settings.First(it => it.key == teleBotTokenKey).value
				: ConfigurationManager.AppSettings[teleBotTokenKey];
			
			return telegramToken;
		}
		
		private void updatePosition(ExecutionReport report)
			=> _queue.Put(
				action =>
				{
					if (report.OrderStatus == OrderStatus.FILLED)
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
					return Task.CompletedTask;
				}
			);
		private void UpdateBalances(List<Balance> newBalances)
			=> _queue.Put(
				async action =>
				{
					lock (_balances)
					{
						if (_balances == null)
						{
							_balances = newBalances;
						} else
						{
							foreach (var item in newBalances)
							{
								if (_balances.Contains(item, Balance.CompareSymbol))
								{
									_balances.RemoveAll(i => i.Currency == item.Currency);
									_balances.Add(item);
								} else
								{
									_balances.Add(item);
								}
							}
						}
					}
					await Task.Yield();
				}
			);
		public override string Name => _name;
		public override IBrokerClient Connector => _client;

		public override void PushSignal(TradingSignal signal)
			=> _queue.Put(
				async action =>
				{
					try
					{
						var order = SignalToOrder(signal);

						if (order == null)
						{
							_logger.Log(LogPriority.Warning, "Received incorrect order", _name);

							return;
						}

						var slaveLeverage = _fullPositionsList.FirstOrDefault(i => i.symbol == signal.Symbol)?.leverage;

						if (slaveLeverage != signal.leverage)
						{
							await _client.SetLeverage(
								new Instrument
								{
									Symbol = signal.Symbol
								}, signal.leverage
							);
						}
						switch (signal.OrderStatus)
						{
						case OrderStatus.NEW:
							if (!_ordersId.Contains(order.ExchangeId))
							{
								_ordersId.Add(order.ExchangeId);
								await _client.SendNewOrder(order);
							}
							break;
						case OrderStatus.CANCELED:
							await _client.CancelOrder(order);
							if (_ordersId.Contains(order.ExchangeId))
							{
								_ordersId.Remove(order.ExchangeId);
							}

							break;
						case OrderStatus.FILLED:
							if (!_ordersId.Contains(order.ExchangeId))
							{
								order.OrderType = OrderType.MARKET;
								await _client.SendNewOrder(order);
								_ordersId.Add(order.ExchangeId);
							}

							//else ordersId.Remove(signal.ClId);
							break;
						}
					} catch (Exception exc)
					{
						_logger.Log(LogPriority.Error, exc, _name, "Exception in BinanceBackend.PushSignal");
						_logger.Log(LogPriority.Debug, exc, _name, "Exception in BinanceBackend.PushSignal");
					}
				}
			);

		private Order SignalToOrder(TradingSignal signal)
		{
			try
			{
				var order = signal.SelfOrder;
				if (signal.OrderStatus == OrderStatus.FILLED)
				{
					if (signal.PositionSide == PositionSide.LONG && signal.OrderSide == OrderSide.SELL)
					{
						var slavePos = _fullPositionsList.FirstOrDefault(i => i.positionSide == signal.PositionSide && i.symbol == signal.Symbol);

						if (slavePos?.positionAmt > 0)
						{
							order.Amount = slavePos.positionAmt;
						}
					} else if (signal.PositionSide == PositionSide.SHORT && signal.OrderSide == OrderSide.BUY)
					{
						var slavePos = _fullPositionsList.FirstOrDefault(i => i.positionSide == signal.PositionSide && i.symbol == signal.Symbol);

						if (slavePos?.positionAmt < 0)
						{
							order.Amount = Math.Abs(slavePos.positionAmt);
						}
					} else
					{
						double tempQty;
						var precision = _instruments.First(i => i.Symbol == order.Symbol).QuantityPrecision;

						if (double.TryParse(Settings.Get("ModeValue"), out tempQty))
						{
							order.Amount = Math.Round(order.Amount * tempQty, (int)precision);
						}
					}
				} else if(signal.OrderStatus == OrderStatus.NEW)
				{
					var precision = _instruments.First(i => i.Symbol == order.Symbol).QuantityPrecision;
					if (signal.CloseAll)
					{
						var slavePos = _fullPositionsList.FirstOrDefault(i => i.positionSide == signal.PositionSide && i.symbol == signal.Symbol);
						order.Amount = slavePos.positionAmt;
					} else
					{

						// if (signal.PositionSide == PositionSide.LONG && signal.OrderSide == OrderSide.SELL)
						// {
						// 	var slavePos = _fullPositionsList.FirstOrDefault(i => i.positionSide == signal.PositionSide && i.symbol == signal.Symbol);
						//
						// 	if (slavePos?.positionAmt > 0)
						// 	{
						// 		var slavePosAmnt = slavePos.positionAmt;
						// 		order.Amount = slavePosAmnt;
						// 	}
						// } else if (signal.PositionSide == PositionSide.SHORT && signal.OrderSide == OrderSide.BUY)
						// {
						// 	var slavePos = _fullPositionsList.FirstOrDefault(i => i.positionSide == signal.PositionSide && i.symbol == signal.Symbol);
						//
						// 	if (slavePos?.positionAmt < 0)
						// 	{
						// 		var slavePosAmnt = slavePos.positionAmt;
						// 		order.Amount = Math.Abs(slavePosAmnt);
						// 	}
						// } else
						// {
						if (Settings.Get("Mode") == "Fixed")
						{

							if (double.TryParse(Settings.Get("ModeValue"), out var tempQty))
							{
								order.Amount = Math.Round(tempQty, (int)precision);
							}
						} else
						{
							if (double.TryParse(Settings.Get("ModeValue"), out var tempQty))
							{
								order.Amount = Math.Round(order.Amount * tempQty, (int)precision);
							}
						}
						// }
					}
				}

				return order;
			} catch (Exception exc)
			{
				_logger.Log(LogPriority.Error, exc, _name, "Exception in BinanceBackend.SignalToOrder");
				_logger.Log(LogPriority.Debug, exc, _name, "Exception in BinanceBackend.SignalToOrder");
				throw;
			}

		}

		private async Task CancelNonMasterOrders(List<Order> openOrders, List<Order> _masterOrders)
		{
			try
			{
				var slaveOpenOrders = openOrders.Where(i => !_masterOrders.Contains(i, Order.CompareClIdAndExchId)).ToList();

				foreach (var order in slaveOpenOrders)
				{
					await _client.CancelOrder(order);

					if (_ordersId.Contains(order.ClientId))
					{
						_ordersId.Remove(order.ClientId);
						_logger.Log(LogPriority.Debug, $"Order '{order.ClientId}' removed from the orderlist", _name);
					}
				}

				_logger.Log(LogPriority.Info, "Non master orders canceled", _name);
			} catch (Exception ex)
			{
				_logger.Log(LogPriority.Error, ex, _name, "Exception in BinanceBackend.CancelNonMasterOrders");
				_logger.Log(LogPriority.Debug, ex, _name, "Exception in BinanceBackend.CancelNonMasterOrders");
			}
		}
		private async Task MasterOrdersSynchro(List<Position> _masterPositions, List<Order> _openOrders, List<Order> _masterOrders)
		{
			try
			{
				var ordersToOpen = _masterOrders.Where(i => !_openOrders.Contains(i, Order.CompareClIdAndExchId)).ToList();
				foreach (var order in ordersToOpen)
				{
					if (!_ordersId.Contains(order.ClientId))
					{
						_ordersId.Add(order.ClientId);
						_logger.Log(LogPriority.Debug, $"Order '{order.ClientId}' added into the orderlist", _name);
					}
					var masterPosition = _masterPositions.FirstOrDefault(i => i.symbol == order.Symbol);

					if (masterPosition != null)
					{
						var slaveLeverage = _fullPositionsList.FirstOrDefault(i => i.symbol == order.Symbol).leverage;

						if (masterPosition.leverage != slaveLeverage)
						{
							await _client.SetLeverage(
								new Instrument
								{
									Symbol = order.Symbol
								}, (uint)masterPosition.leverage
							);
						}
					}

					var newOrder = (Order)order.Clone();
					var precision = _instruments.First(i => i.Symbol == order.Symbol).QuantityPrecision;
					if (Settings.Get("Mode") == "Fixed")
					{

						if (double.TryParse(Settings.Get("ModeValue"), out var tempQty))
						{
							newOrder.Amount = Math.Round(tempQty, (int)precision);
						}
					} else
					{
						if (double.TryParse(Settings.Get("ModeValue"), out var tempQty))
						{
							newOrder.Amount = Math.Round(order.Amount * tempQty, (int)precision);
						}
					}
					await _client.SendNewOrder(newOrder);
				}

				_logger.Log(LogPriority.Info, "Open orders synchronized with master", _name);
			} catch (Exception ex)
			{
				_logger.Log(LogPriority.Error, ex, _name, "Exception in BinanceBackend.MasterOrdersSynchro");
				_logger.Log(LogPriority.Debug, ex, _name, "Exception in BinanceBackend.MasterOrdersSynchro");
			}
		}
		public override bool AllowChangeSettings()
			=> _fullPositionsList.Where(i => i.positionAmt != 0).ToList().Count == 0;
		private async Task PositionEqualizer(List<Position> _masterPositions, SortedSet<Position> _positions)
		{
			try
            {
                var slavePositionsList = _positions.Where(i => _masterPositions.Contains(i, new Position())).ToList();

                foreach (var slavePosition in slavePositionsList)
                {
                    var id = "snh_" + Guid.NewGuid().ToString("N");
                    double tempQty;
                    double coefQty;
                    var precision = _instruments.First(i => i.Symbol == slavePosition.symbol).QuantityPrecision;
                    var masterPosition = _masterPositions.FirstOrDefault(i =>
                        i.symbol == slavePosition.symbol && i.positionSide == slavePosition.positionSide);
                    if (masterPosition != null)
                    {
                        _logger.Log(LogPriority.Debug,
                            $"PositionEqualizer master position: Symbol({masterPosition.symbol}), PositionSide({masterPosition.positionSide}), PositionAmnt({masterPosition.positionAmt})",
                            _name);
                        _logger.Log(LogPriority.Debug,
                            $"PositionEqualizer slave position: Symbol({slavePosition.symbol}), PositionSide({slavePosition.positionSide}), PositionAmnt({slavePosition.positionAmt})",
                            _name);
                        if (double.TryParse(Settings.Get("ModeValue"), out tempQty))
                        {
                            coefQty = Math.Round(masterPosition.positionAmt * tempQty, (int)precision);
                        }
                        else
                        {
                            coefQty = masterPosition.positionAmt;
                        }
                        _logger.Log(LogPriority.Debug,
                            $"PositionEqualizer coefQty = ({coefQty}))",
                            _name);

                        if (slavePosition.positionAmt > coefQty && slavePosition.positionSide == PositionSide.LONG)
                        {
                            double newQty;
                            newQty = Math.Round(slavePosition.positionAmt - coefQty, (int)precision);
                            _logger.Log(LogPriority.Debug,
                                $"PositionEqualizer newQty = ({newQty}))",
                                _name);
                            var order = new Order
                            {
                                Amount = newQty,
                                OrderType = OrderType.MARKET,
                                Symbol = slavePosition.symbol,
                                OrderSide = OrderSide.SELL,
                                PositionSide = PositionSide.LONG,
                                ClientId = id
                            };

                            await _client.SendNewOrder(order);
                        }

                        if (slavePosition.positionAmt < coefQty && slavePosition.positionSide == PositionSide.SHORT)
                        {
                            double newQty;
                            newQty = Math.Round(slavePosition.positionAmt - coefQty, (int)precision);
                            _logger.Log(LogPriority.Debug,
                                $"PositionEqualizer newQty = ({newQty}))",
                                _name);
                            var order = new Order
                            {
                                Amount = Math.Abs(newQty),
                                OrderType = OrderType.MARKET,
                                Symbol = slavePosition.symbol,
                                OrderSide = OrderSide.BUY,
                                PositionSide = PositionSide.SHORT,
                                ClientId = id
                            };

                            await _client.SendNewOrder(order);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.Log(LogPriority.Error,
                    exc, _name, "Exception in BinanceBackend.PositionEqualizer");
                _logger.Log(LogPriority.Debug,
					exc, _name, "Exception in BinanceBackend.PositionEqualizer");
            }
		}
		private async Task CloseNonMasterPositions(List<Position> _masterPositions, SortedSet<Position> _positions)
		{
			try
			{
				var slavePositionsList = _positions.Where(i => !_masterPositions.Contains(i, new Position())).ToList();

				foreach (var pos in slavePositionsList)
				{
					var id = "snh_" + Guid.NewGuid().ToString("N");
					if (pos.positionAmt > 0)
					{
						var order = new Order
						{
							Price = 0,
							Amount = pos.positionAmt,
							OrderType = OrderType.MARKET,
							Symbol = pos.symbol,
							OrderSide = OrderSide.SELL,
							PositionSide = PositionSide.LONG,
							ClientId = id
						};
						await _client.SendNewOrder(order);
					} else if (pos.positionAmt < 0)
					{
						var order = new Order
						{
							Price = 0,
							Amount = Math.Abs(pos.positionAmt),
							OrderType = OrderType.MARKET,
							Symbol = pos.symbol,
							OrderSide = OrderSide.BUY,
							PositionSide = PositionSide.SHORT,
							ClientId = id
						};
						await _client.SendNewOrder(order);
					}
				}

				if (slavePositionsList.Count >= 1)
				{
					_logger.Log(LogPriority.Info, "Non master positions closed", _name);
				} else
				{
					_logger.Log(LogPriority.Info, "No position to close", _name);
				}
			} catch (Exception ex)
			{
				_logger.Log(LogPriority.Error, ex, _name, "Exception in BinanceBackend.CloseNonMasterPositions");
				_logger.Log(LogPriority.Debug, ex, _name, "Exception in BinanceBackend.CloseNonMasterPositions");
			}
		}
		public override Task CloseAllPositions()
		{
			_queue.Put(
				async action =>
				{
					try
					{
						var positions = (await _client.GetPositions()).Where(i => i.positionAmt != 0).ToArray();
						foreach (var pos in positions)
						{
							if (pos.positionAmt > 0)
							{
								var order = new Order
								{
									Price = 0,
									Amount = pos.positionAmt,
									OrderType = OrderType.MARKET,
									Symbol = pos.symbol,
									OrderSide = OrderSide.SELL,
									PositionSide = PositionSide.LONG
								};

								await _client.SendNewOrder(order);
							} else if (pos.positionAmt < 0)
							{
								var order = new Order
								{
									Price = 0,
									Amount = Math.Abs(pos.positionAmt),
									OrderType = OrderType.MARKET,
									Symbol = pos.symbol,
									OrderSide = OrderSide.BUY,
									PositionSide = PositionSide.SHORT
								};

								await _client.SendNewOrder(order);
							}
						}

						if (positions.Any())
						{
							_logger.Log(LogPriority.Info, "All positions closed", _name);
						} else
						{
							_logger.Log(LogPriority.Info, "No position to close", _name);
						}
					} catch (Exception ex)
					{
						_logger.Log(LogPriority.Error, ex, _name, "Exception in BinanceBackend.CloseAllPositions");
						_logger.Log(LogPriority.Debug, ex, _name, "Exception in BinanceBackend.CloseAllPositions");
					}
				}
			);

			return Task.CompletedTask;
		}
		public override Task SynchronizeOrders()
		{
			_queue.Put(
				async action =>
				{
					try
					{
						var _openOrders = await _client.GetAllOpenOrderds();
						var _masterPositions = _masterConnector.MasterPositions;
						var _masterOrders = _masterConnector.MasterOrders;
						await CancelNonMasterOrders(_openOrders, _masterOrders);
						if (Settings.Get("CopyMasterOrders") == "true")
						{
							await MasterOrdersSynchro(_masterPositions, _openOrders, _masterOrders);
						}
					} catch (Exception ex)
					{
						_logger.Log(LogPriority.Error, ex, _name, "Exception in BinanceBackend.SynchronizeOrders");
						_logger.Log(LogPriority.Debug, ex, _name, "Exception in BinanceBackend.SynchronizeOrders");
					}
				}
			);

			return Task.CompletedTask;
		}
		public override Task SynchronizePositions()
		{
			_queue.Put(
				async action =>
				{
					try
					{
						var _masterPositions = _masterConnector.MasterPositions;
						var accountInfo = await _client.GetAccountInfo();
						var _positions = new SortedSet<Position>(accountInfo.Item1.Where(i => i.positionAmt != 0).ToList());
						await CloseNonMasterPositions(_masterPositions, _positions);
						await PositionEqualizer(_masterPositions, _positions);
					} catch (Exception ex)
					{
						_logger.Log(LogPriority.Error, ex, _name, "Exception in BinanceBackend.SynchronizePositions");
						_logger.Log(LogPriority.Debug, ex, _name, "Exception in BinanceBackend.SynchronizePositions");
					}
				}
			);

			return Task.CompletedTask;
		}
		public override Task CancelAllOrders()
		{
			_queue.Put(
				async action =>
				{
					try
					{
						foreach (var i in _instruments)
						{
							await _client.CancelAllOrders(i.Symbol);
						}

						_logger.Log(LogPriority.Info, "All open orders was canceled", _name);
					} catch (Exception ex)
					{
						_logger.Log(LogPriority.Error, ex, _name, "Exception in BinanceBackend.CancelAllOrders");
						_logger.Log(LogPriority.Debug, ex, _name, "Exception in BinanceBackend.CancelAllOrders");
					}
				}
			);

			return Task.CompletedTask;
		}
		private void HandleLogon(ConnectionEvent ev)
			=> _queue.Put(
				async action =>
				{
					if (ev == Shared.Models.ConnectionEvent.Logon)
					{
						if (_masterConnector.Connector.IsConnected == true && _client.IsConnected == true)
						{
							try
							{
								var accountInfo = await _client.GetAccountInfo();
								_fullPositionsList = accountInfo.Item1;
								lock (_balances)
								{
									_balances = accountInfo.Item2;
								}
								var _positions = new SortedSet<Position>(accountInfo.Item1.Where(i => i.positionAmt != 0).ToList());
								var _openOrders = await _client.GetAllOpenOrderds();
								var _masterPositions = _masterConnector.MasterPositions;
								var _masterOrders = _masterConnector.MasterOrders;
								await CancelNonMasterOrders(_openOrders, _masterOrders);
								await CloseNonMasterPositions(_masterPositions, _positions);
								await PositionEqualizer(_masterPositions, _positions);

								if (Settings.Get("CopyMasterOrders") == "true")
								{
									await MasterOrdersSynchro(_masterPositions, _openOrders, _masterOrders);
								}
							} catch (Exception ex)
							{
								_logger.Log(LogPriority.Error, ex, Name, "Exception in BinanceBackEnd.HandleLogon. Synchronization was not execute. Try to restart this connector.");
								_logger.Log(LogPriority.Debug, ex, Name, "Exception in BinanceBackEnd.HandleLogon. Synchronization was not execute. Try to restart this connector.");
							}
						}
					}
				}
			);

		public override string GetBalanceValue(WalletCurrency currency)
		{
			if (_balances != null)
			{
				lock (_balances)
				{
					try
					{
						if (_balances.Contains(
							new Balance
							{
								Currency = currency.ToString()
							}, Balance.CompareSymbol
						))
						{
							var value = _balances.First(item => item.Currency == currency.ToString()).Value;

							return value.ToString();
						}

						return $"No {currency} wallet";
					} catch (Exception exc)
					{
						_logger.Log(LogPriority.Error, exc, _name, "Exception in BinanceBackend.GetBalanceValue");
						_logger.Log(LogPriority.Debug, exc, _name, "Exception in BinanceBackend.GetBalanceValue");

						return "Error";
					}
				}
			}

			return "Null";
		}
		public override Task Start()
		{
			_queue.Start();
			_queue.Put(
				async action =>
				{
					if (_isRunning)
					{
						return;
					}

					try
					{
						_masterConnector.ConnectionEvent += HandleLogon;
						ConnectionEvent += HandleLogon;
						await _client.StartAsync();
						_instruments = await _client.GetInstruments();
						await _client.ChangePositionMode(true);
						_isRunning = true;
					} catch (Exception ex)
					{
						_logger.Log(LogPriority.Error, ex, _name, "Exception in BinanceBackend.Start");
						_logger.Log(LogPriority.Debug, ex, _name, "Exception in BinanceBackend.Start");
					}
				}
			);

			return Task.CompletedTask;
		}

		public override void Stop()
		{
			_queue.Put(
				async action =>
				{
					if (_isRunning)
					{
						_masterConnector.ConnectionEvent -= HandleLogon;
						ConnectionEvent -= HandleLogon;
						_client.Stop();
						_isRunning = false;
					}

					await Task.Yield();
				}
			);
			_queue.Stop();
		}
		public override void ChangeLeverage((string, int) param)
			=> _queue.Put(
				async action =>
				{
					try
					{
						await _client.SetLeverage(
							new Instrument
							{
								Symbol = param.Item1
							}, (uint)param.Item2
						);
					} catch (Exception ex)
					{
						_logger.Log(LogPriority.Error, ex, Name, "Exception in BinanceBackEnd.ChangeLeverag");
						_logger.Log(LogPriority.Debug, ex, Name, "Exception in BinanceBackEnd.ChangeLeverag");
					}
					await Task.Yield();
				}
			);

		private void UpdateLeverage((string, int) param)
			=> _queue.Put(
				async action =>
				{
					_fullPositionsList.FindAll(i => i.symbol == param.Item1).ForEach(j => j.leverage = param.Item2);
					await Task.Yield();
				}
			);

	}

}
