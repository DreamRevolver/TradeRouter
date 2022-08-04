using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using CommonExchange;

using Newtonsoft.Json.Linq;

using Shared.Broker;
using Shared.Interfaces;
using Shared.Models;

using SharedBinance.interfaces;
using SharedBinance.Models;
using SharedBinance.Models.BinanceReports;

namespace BinanceFuturesConnector
{

	public sealed class PublicSubscribe
	{

		private string _id;
		internal Instrument _pair;
		private readonly SubscriptionModel _model;
		public int lvl;
		public string _endpoint;
		private IDataStream _chnlTicker;
		private IDataStream _chnlTopBook;
		private readonly BinanceClient _client;
		private readonly ILogger _logger;
		private ISettingStorage _settings;
		private readonly string _name;
		private readonly string _uri;


		public PublicSubscribe(ILogger logger, BinanceClient client, ISettingStorage settings, string name, SubscriptionModel model)
		{
			_client = client;
			_logger = logger;
			_settings = settings;
			_model = model;
			_uri = settings.Get("Wss");
			_name = name;
		}

		public async void Start(Instrument pair)
		{
			_pair = pair;
			if (_model == SubscriptionModel.FullBook)
			{
				var uriTopBook = "";
				_id = pair.Id();
				//<symbol>@depth<levels>@100ms
				uriTopBook = $"{_uri}/ws/{Utilities.EndpointSymbol(pair)}@depth5@500ms";
				_chnlTopBook = new WebSocketWrapper(uriTopBook, _id, _logger, _name);
				await _chnlTopBook.Start(ParseMessage);
			}
			else if (_model == SubscriptionModel.TopBook)
			{
				var uriTicker = "";
				_id = pair.Id();
				//<symbol>@bookTicker
				uriTicker = $"{_uri}/ws/{Utilities.EndpointSymbol(pair)}@bookTicker";
				_chnlTicker = new WebSocketWrapper(uriTicker, _id, _logger, _name);
				await _chnlTicker.Start(ParseMessage);
			}
		}

		public async void Stop()
		{

			if (_chnlTicker != null)
			{ await _chnlTicker.Stop(); }
			if (_chnlTopBook != null)
			{ await _chnlTopBook.Stop(); }
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
						_logger.Log(LogPriority.Warning, $"<--Report : Empty message received - '{jsonResponse}' : ", _name);
						return;
					}

					var jToken = JObject.Parse(jsonResponse);
					var type = "";
					if (jToken["e"] != null)
					{
						type = jToken["e"].Value<string>();
					}

					if (type == "depthUpdate")
					{
						var time = Utilities.ConvertFromUnixTimestamp(jToken["E"].Value<long>());
						var report = jToken.ToObject<DepthStreamReport>();

						var askList = new List<MarketUpdate>();
						for (var i = 0; i < report.askUpdates.Length; i++)
						{
							askList.Add(new MarketUpdate
							{
								Action = UpdateAction.New,
								Type = EntryType.Bid,
								Price = report.askUpdates[i][0],
								Volume = report.askUpdates[i][1]
							});
						}

						var bidList = new List<MarketUpdate>();
						for (var i = 0; i < report.bidUpdates.Length; i++)
						{
							bidList.Add(new MarketUpdate
							{
								Action = UpdateAction.New,
								Type = EntryType.Offer,
								Price = report.bidUpdates[i][0],
								Volume = report.bidUpdates[i][1]
							});
						}

						var depthSnapshot = new DepthSnapshot
						{
							asks = askList,
							bids = bidList,
							lastTimeUpdate = time
						};

						_client.DispatchMarketDataHandler(_name, _pair.Symbol, new MarketBook(), depthSnapshot, null);
					}
					else if (type == "bookTicker")
					{
						var report = jToken.ToObject<TickerReport>();
						_client.DispatchMarketDataHandler(_name, _pair.Symbol,
							new MarketBook
							{
								Bid = report.BidPx,
								Ask = report.AskPx
							}, new DepthSnapshot(), null);
						// _logger.Log(LogPriority.Info, $"<--bookTicker : Symbol: {_pair.Symbol}, Bid: {report.BidPx}, BidSize: {report.BidQty}, " +
						//     $"Ask: {report.AskPx}, AskSize: {report.AskQty}", _name);
					}
				}
					break;
				case StreamMessageType.Logon:
					break;
				case StreamMessageType.Error:
					break;
				}
			}
			catch (Exception ex)
			{
				_logger.Log(LogPriority.Error, $"Exception catched from ParseMessage | {ex.Message}", _name);
				throw;
			}
			await Task.Yield();
		}
	}

}
