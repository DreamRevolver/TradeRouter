using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using CommonExchange;
using Utf8Json;
using Shared.Broker;
using Shared.Interfaces;
using Shared.Models;

using SharedBinance.interfaces;
using SharedBinance.Models;
using SharedBinance.Services;

namespace BinanceFuturesConnector
{

	public sealed class PrivateChannel
	{

		public IDataStream channel;
		private readonly BinanceClient _client;
		private readonly ILogger _logger;
		private readonly string _name;
		private readonly string _uri;
		private string _listenKey;
		private readonly RestClient _rest;

		private readonly string _apiKey;
		private readonly string _apiSecret;

		private readonly IActionScheduler _scheduler;
		
		public PrivateChannel(
			ILogger logger, 
			BinanceClient client, 
			ISettingStorage settings, 
			string name, 
			ThrottlingRoundRobinScheduler? invoker = null)
		{
			Debug.Assert(logger != null && client != null);
			_scheduler = invoker is null ? ThrottlingRoundRobinScheduler.Instance : invoker;
			_client = client;
			_logger = logger;
			_uri = settings.Get("Wss");
			_name = name;
			_apiKey = settings.Get("APIKey");
			_apiSecret = settings.Get("APISecret");
			_rest = new RestClient(_logger, settings, _name);
		}

		private IDisposable _subscriptionDisposer;

		private async Task KeepAlive()
		{
			//_logger.Log(LogPriority.Debug, "keep alive user channel", _name);
			var _lastListenKey = _listenKey;

			await RefreshListenKey();

			if (string.IsNullOrEmpty(_listenKey))
			{
				_logger.Log(LogPriority.Debug, "listen key is null", _name);

				//  REST request count limit was reached
				return;
			}

			await PingListenKey();

			if (_listenKey != _lastListenKey)
			{
				_logger.Log(LogPriority.Debug, "listen key was updated", _name);
				channel?.Stop();
				channel = new WebSocketWrapper($"{_uri}/ws/{_listenKey}", "UserChnl", _logger, _name);
				await channel.Start(ParseMessage);
			}

		}

		private async void KeepAliveAsync()
		{
			try
			{
				await KeepAlive().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				_logger.Log(LogPriority.Error,ex, _name, "KeepAliveAsync Exception: ");
			}
		}

		public async Task Start()
		{
			_logger.Log(LogPriority.Debug, "START connector", _name);

			if (_subscriptionDisposer != null)
			{
				throw new Exception("already started");
			}

			await KeepAlive();
			_subscriptionDisposer = _scheduler.Schedule(KeepAliveAsync);
		}

		public async Task Stop()
		{
			_logger.Log(LogPriority.Debug, "STOP connector", _name);

			if (_subscriptionDisposer is null)
			{
				throw new Exception("already stopped");
			}

			await (channel?.Stop() ?? Task.CompletedTask);
			_subscriptionDisposer.Dispose();
		}

		private async Task ParseMessage(SocketEventHandlerArgs msg)
		{
			try
			{
				switch (msg.type)
				{
				case StreamMessageType.Data:
				{
					var jsonResponse = Encoding.UTF8.GetString(msg.msg, 0, msg.msg.Length);

					if (string.IsNullOrEmpty(jsonResponse))
					{
						_logger.Log(LogPriority.Warning, $"<-- REPORT : Empty message received - '{jsonResponse}' : ", _name);

						return;
					}

					var socketEvent = JsonSerializer.Deserialize<BinanceSteamEventDTO>(jsonResponse);
					var eventTime = socketEvent.E;
					switch (socketEvent.e)
					{
					case "ORDER_TRADE_UPDATE":
					{
						var report = socketEvent.CreateReport();
						_logger.Log(
							LogPriority.Info,
							$"<-- ORDER | {report.OrderStatus} | {report.Symbol} | {report.OrderSide} | {report.PositionSide} | {report.OrderType} | P:{report.Price} | Q:{report.Amount} | #{report.ExchangeId} / #{report.ClientId} | " +
							$"StopPrice:{report.StopPrice} | AvgPrice:{report.AvgPx} | LastFilledPrice:{report.LastFilledPrice} | " +
							$"FilledQty:{report.FilledQty} | ClosePosition: {report.ClosePosition}", _name
						);
						_client.DispatchExecutionReportHandler(
							_name,
							report
						);
						break;
					}
					case "ACCOUNT_UPDATE":
					{
						var positionsList = new List<Position>();
						var balanceList = new List<Balance>();
						positionsList = socketEvent.a.P.Where(i => i.ps != "BOTH").Select(i => i.Convert(eventTime)).ToList();

						foreach (var pos in positionsList)
						{
							_logger.Log(LogPriority.Debug, $"<-- POSITION | UPDATE | {pos.symbol} | {pos.positionSide} | {pos.positionAmt} | Time: {eventTime}", _name);
						}

						_client.DispatchPositionUpdateHandler(socketEvent.e, positionsList);
						balanceList = socketEvent.a.B.Select(i => i.Convert(eventTime)).ToList();

						foreach (var bal in balanceList)
						{
							_logger.Log(LogPriority.Info, $"<-- BALANCE | UPDATE | {bal.Currency} | {bal.Value} | Change except PnL and Commission: {bal.BalanceChange} | Event type: {eventTime}", _name);
						}

						_client.DispatchBalanceUpdateHandler(socketEvent.e, balanceList);

						break;
					}
					case "ACCOUNT_CONFIG_UPDATE":
					{
						if (socketEvent.ac != null)
							_client.DispatchAccountConfigUpdateHandler((socketEvent.ac.Value.s, socketEvent.ac.Value.l));
						break;
					}
					case "MARGIN_CALL":
					{

						break;
					}
					case "listenKeyExpired":
					{

						break;
					}
					}
				}

					break;
				case StreamMessageType.Error:
					_client.DispatchBrokerEventHandler(_name, BrokerEvent.InternalError, "");

					break;
				case StreamMessageType.Logon:
					_client.DispatchBrokerEventHandler(_name, BrokerEvent.SessionLogon, "");

					break;
				case StreamMessageType.Logout:
					_client.DispatchBrokerEventHandler(_name, BrokerEvent.SessionLogout, "");

					break;
				}
			} catch (Exception ex)
			{
				_logger.Log(LogPriority.Debug, ex, _name, "Parsing error");

				throw;
			}

			await Task.Yield();
		}

		private async Task PingListenKey()
			=> await _rest.SendRequest("/fapi/v1/listenKey", HttpMethod.Put);

		private async Task RefreshListenKey()
		{
			if (_apiKey == string.Empty || _apiSecret == string.Empty)
			{
				_logger.Log(LogPriority.Error, "ApiKey or ApiSecret is not defined", _name);

				throw new Exception("key error");
			}

			var msg = await _rest.SendRequest("/fapi/v1/listenKey", HttpMethod.Post);
			var res = new BinanceResponseDto<ListenKeyDTO>(msg);
			if (res.error is null)
			{
				_listenKey = res.data.listenKey;
			} else
			{
				_listenKey = null;
			}
		}

	}

}
