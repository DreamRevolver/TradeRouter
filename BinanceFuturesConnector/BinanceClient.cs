using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;
using Shared.Broker;
using Shared.Interfaces;
using Shared.Models;
using SharedBinance.interfaces;
using SharedBinance.Models;
using Utf8Json;
namespace BinanceFuturesConnector
{
    public class BinanceClient : IBrokerClient
    {

        private readonly Dictionary<string, PublicSubscribe> _subscibeList = new Dictionary<string, PublicSubscribe>();
        private PrivateChannel _userChannel;
        private List<string> _pairs = new List<string>();
        private static Task<IEnumerable<Instrument>> _getInstrumentsTask;
        private List<Instrument> _instrumentList;
        private readonly ILogger _logger;
        private readonly ISettingStorage _settings;
        private readonly RestClient _rest;
        private readonly AsyncLock _lock = new AsyncLock();
        public event Action<string, BrokerEvent, string> BrokerEventHandler = delegate { };
        public event Action<string, ExecutionReport> ExecutionReportHandler = delegate { };
        public event Action<(string, int)> AccountConfigUpdateHandler = delegate { };
        public event Action<string, List<Position>> PositionUpdateHandler = delegate { };
        public event Action<string, List<Balance>> BalanceUpdateHandler = delegate { };
        public event Action<string, Order> OrderHandler = delegate { };
        public event Action<string, string, MarketBook, DepthSnapshot, IEnumerable<MarketUpdate>> MarketDataHandler = delegate { };
        public BinanceClient(string name, ILogger logger, ISettingStorage settings)
        {
            _logger = logger;
            _settings = settings;
            var apiKey = settings.Get("APIKey");
            var apiSecret = settings.Get("APISecret");
            settings.Get("Url");
            settings.Get("Wss");
            Name = name;
            RetryHelper.Set(logger, settings.Get("RetryAttempts"),settings.Get("RetryDelayMs"));

            _logger.Log(LogPriority.Debug, "Create new connector", Name);
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.Log(LogPriority.Warning, "Channel has not been started : apiKey  Cannt Be Null Or Empty", Name);
            }
            if (string.IsNullOrEmpty(apiSecret))
            {
                _logger.Log(LogPriority.Warning, "Channel has not been started : apiSecret  Cannt Be Null Or Empty", Name);
            }

            _rest = new RestClient(_logger, _settings, Name, ProxyWrapper.GetHTTPClient(Name));
        }
        public void DispatchBrokerEventHandler(string name, BrokerEvent what, string details)
            => BrokerEventHandler(name, what, details);

        public void DispatchExecutionReportHandler(string name, ExecutionReport report)
            => ExecutionReportHandler(name, report);

        public void DispatchAccountConfigUpdateHandler((string, int) param)
            => AccountConfigUpdateHandler(param);

        public void DispatchPositionUpdateHandler(string name, List<Position> positions)
            => PositionUpdateHandler(name, positions);

        public void DispatchBalanceUpdateHandler(string name, List<Balance> balances)
            => BalanceUpdateHandler(name, balances);

        public void DispatchOrderHandler(string name, Order order)
            => OrderHandler(name, order);

        public void DispatchMarketDataHandler(string name, string symbol, MarketBook topOfBook, DepthSnapshot snapshot, IEnumerable<MarketUpdate> updates)
            => MarketDataHandler(name, symbol, topOfBook, snapshot, updates);
        public string Name { get; }

        public async Task StartAsync()
        {
            if (IsStarted != true)
            {
                IsStarted = true;
                _logger.Log(LogPriority.Debug, $"Start {Name} connector", Name);
                BrokerEventHandler(Name, BrokerEvent.ConnectorStarted, string.Empty);
            }

            await Task.Run(async () =>
            {
                try
                {
                    if (_pairs.Count == 0)
                    {
                        _instrumentList = await GetInstruments();
                        _pairs = _instrumentList.Select(Utilities.BinanceSymbol).ToList();
                    }
                    if (_subscibeList.Count > 0)
                    {
                        foreach (var i in _subscibeList)
                        {
                            i.Value.Start(i.Value._pair);
                        }
                    }

                    _userChannel = new PrivateChannel(_logger, this, _settings, Name);
                    await _userChannel.Start();
                }
                catch (Exception ex)
                {
                    _logger.Log(LogPriority.Debug, ex, Name, "Channel has not been started, please restart channel manually");
                    throw;
                }
            });
        }

        public async void Stop()
        {
            if (_userChannel != null)
            { await _userChannel.Stop(); }
            
            using (await _lock.LockAsync())
            {
                if (_subscibeList.Count > 0)
                {
                    foreach (var i in _subscibeList)
                    {
                        i.Value.Stop();
                    }
                }
            }
            IsStarted = false;
            _logger.Log(LogPriority.Debug, $"Stop {Name} connector", Name);
            BrokerEventHandler(Name, BrokerEvent.ConnectorStopped, string.Empty);
        }
        public bool? IsStarted { get; private set; } = false;

        public bool? IsConnected
            => _userChannel?.channel?.IsConnected == true;


        public async Task<List<Instrument>> GetInstruments()
        {
            if (_instrumentList == null)
            {
                _logger.Log(LogPriority.BinanceWeight, $"GetInstruments - Weight: 1", Name);
                var msg = await _rest.SendRequest("/fapi/v1/exchangeInfo", HttpMethod.Get);
                var result = new BinanceResponseDto<ExchInfoDTO>(msg);
                if (result.error is null)
                {
                    _instrumentList = result.data.symbols.Select(i => i.Convert()).ToList();
                } else
                {
                    _logger.Log(LogPriority.Error, $"<-- REPORT | ERROR | FAILED TO GET INSTRUMENTS | ErrorCode: {result.error.Value.code}, ErrorMessege: {result.error.Value.msg}", Name);
                    throw new Exception($"FAILED TO GET INSTRUMENTS | {result.error.Value.msg}");
                }
            }
            return _instrumentList;
        }

        #region Order commands

        public async Task<Order> CancelOrder(Order order)
        {
            try
            {
                var param = $"symbol={Utilities.BinanceSymbol(order)}";
                if (!String.IsNullOrEmpty(order.ClientId))
                {
                    param += $"&origClientOrderId={order.ClientId}";
                }
                else
                {
                    param += $"&orderId={order.ExchangeId}";
                }
                var msg = await _rest.SendRequest("/fapi/v1/order", HttpMethod.Delete, true, param);
                var result = new BinanceResponseDto<CanceledOrderDTO>(msg);
                if (result.error is null)
                {
                    _logger.Log(LogPriority.Info, $"--> ORDER | CANCEL | {order.Symbol} | {order.OrderSide} | {order.PositionSide} | {order.OrderType} | P: {order.Price} | Q: {order.Amount} | #{order.ClientId} | Response: {msg}", Name);
                    _logger.Log(LogPriority.BinanceWeight, $"CancelOrder - Weight: 1", Name);
                } else
                {
                    _logger.Log(LogPriority.Error, $"<-- ORDER | CANCEL REJECTED | #{order.ExchangeId} / #{order.ClientId} | ErrorCode: {result.error.Value.code}, ErrorMessege: {result.error.Value.msg}", Name);
                }
                return order;
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "Failed to CancelOrder");
                _logger.Log(LogPriority.Debug, ex, Name, "Failed to CancelOrder");
                throw new Exception("Failed to cancel order. See inner exception for more details.", ex);
            }
        }
        public async Task CancelAllOrders(string symbol)
        {
            try
            {
                var param = $"symbol={symbol}";
                var msg = await _rest.SendRequest("/fapi/v1/allOpenOrders", HttpMethod.Delete, true, param);
                var result = new BinanceResponseDto<OrderDTO>(msg);
                if (result.error is null)
                {
                    _logger.Log(LogPriority.Debug, $"--> REQUEST | CANCEL ALL OPEN ORDERS | SYMBOL = {symbol}", Name);
                    _logger.Log(LogPriority.BinanceWeight, $"CancelAllOrders - Weight: 1", Name);
                } else
                {
                    _logger.Log(LogPriority.Warning, $"All open orders with symbol = {symbol} not canceled!!! | ErrorCode: {result.error.Value.code}, ErrorMessege: {result.error.Value.msg}", Name);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, $"Failed to cancel all orders with symbol = {symbol}");
                _logger.Log(LogPriority.Debug, ex, Name, $"Failed to cancel all orders with symbol = {symbol}");
                throw new Exception($"Failed to cancel all orders with symbol = {symbol}. See inner exception for more details.",ex);
            }

        }
        public Task<Order> ModifyOrder(Order order, double price, double stopPrice, uint qty)
            => throw new NotSupportedException("Replaced functionality does not supported at the moment.");
    #endregion
        public async Task<Order> SendNewOrder(Order order)
        {
            try
            {
                if (order.ExchangeId == null)
                {
                    order.ExchangeId = Guid.NewGuid().ToString();
                }
                var param = Utilities.BinanceOrder(order);
                var result = RetryHelper.ExecuteRetry(() => _rest.SendRequestFull("/fapi/v1/order", HttpMethod.Post, true, param), order, Name).Result;
                if (result.error != null)
                {
                    _logger.Log(LogPriority.Error, $"<-- ORDER | ORDER REJECTED | #{order.ExchangeId} / #{order.ClientId} | Binance error: {result.error.Value.msg} | code: {result.error.Value.code}", Name);
                }
                return order;
            } catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, $"Failed to send new order");
                _logger.Log(LogPriority.Debug, ex, Name, $"Failed to send new order");
                throw new Exception("Failed to send new order. See inner exception for more details.",ex);
            }
        }
        public async Task<List<Order>> GetAllOpenOrderds()
        {
            string msg;
            var result = new List<Order>();
            try
            {
                msg = await _rest.SendRequest("/fapi/v1/openOrders", HttpMethod.Get, true);
                _logger.Log(LogPriority.BinanceWeight, $"GetAllOpenOrderds - Weight: 40", Name);
                var res = new BinanceResponseDto<List<OpenOrderDTO>>(msg);
                if (res.error is null)
                {
                    result = res.data.Select(i => i.Convert()).ToList();
                } else
                {
                    _logger.Log(LogPriority.Error, $"Failed to GetAllOpenOrders. ErrorCode: {res.error.Value.code}, ErrorMessege: {res.error.Value.msg}", Name);
                }
            }
            catch (Exception exc)
            {
                _logger.Log(LogPriority.Error, exc, Name, "Exception in BinanceApp.GetAllOpenOrders");
                _logger.Log(LogPriority.Debug, exc, Name, "Exception in BinanceApp.GetAllOpenOrders");
                throw new Exception("Failed to get all open orders. See inner exception for more details.", exc);
            }
            return result;
        }

    #region Subscribtion events


        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public async Task Subscribe(Instrument instr, SubscriptionModel model)
        {
            if (_pairs.Count == 0)
            {
                try
                {
                    var _tmp_list = await GetInstruments();
                    if (_tmp_list == null || !_tmp_list.Any())
                    {
                        throw new Exception("Subscribe: Сan not download the list of instruments");
                    }
                    _pairs = _tmp_list.Select(u => Utilities.BinanceSymbol(u)).ToList();
                }
                catch (Exception ex)
                {
                    _logger.Log(LogPriority.Error, ex, Name, "Exception in BinanceApp.Subscribe");
                    _logger.Log(LogPriority.Debug, ex, Name, "Exception in BinanceApp.Subscribe");
                    BrokerEventHandler(Name, BrokerEvent.CoinSubscribedFault, instr.Id());
                    throw new Exception("Exception in BinanceApp.Subscribe. See inner exception for more details.", ex);
                }
            }

            if (instr == null)
            {
                _logger.Log(LogPriority.Error, "Subscribe: Coin is not specified", Name);
                BrokerEventHandler(Name, BrokerEvent.CoinSubscribedFault, instr.Id());
                throw new ArgumentNullException("instr");
            }

            if (instr.Symbol != "All" && _pairs.Where(u => u == Utilities.BinanceSymbol(instr)).Count() == 0)
            {
                _logger.Log(LogPriority.Error, $"Subscribe: Coin '{instr.Id()}' is not valid", Name);
                BrokerEventHandler(Name, BrokerEvent.CoinSubscribedFault, instr.Id());
                throw new Exception($"Coin '{instr.Id()}' is not valid");
            }


            //if (IsConnected != true)
            //{
            //    _logger.Log(LogPriority.Error, $"Subscribe: {Name} is not connected", Name);
            //    _client.OnEvent(Name, BrokerEvent.CoinSubscribedFault, instr.Id());
            //}

            try
            {
                using (await _lock.LockAsync())
                {
                    var id = instr.Id();
                    if (!_subscibeList.ContainsKey(id))
                    {
                        var sub = new PublicSubscribe(_logger, this, _settings, Name, model);
                        sub.Start(instr);
                        _subscibeList.Add(id, sub);
                    }
                    //var i = _subscibeList[id];
                    //i.Start(instr);
                }
                _logger.Log(LogPriority.Debug, $"Subscribe {instr.Id()} {model} chnl", Name);
                //_client.OnEvent(Name, BrokerEvent.CoinSubscribed, instr.Id());
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "BinanceApp.Subscribe Connection error");
                _logger.Log(LogPriority.Debug, ex, Name, "BinanceApp.Subscribe Connection error");
                throw new Exception("BinanceApp.Subscribe Connection error. See inner exception for more details.", ex);
                //_client.OnEvent(Name, BrokerEvent.CoinSubscribedFault, instr.Id());
            }
        }
        public async Task Unsibscribe(Instrument instr)
        {
            using (await _lock.LockAsync())
            {
                var subscribe = _subscibeList.Where(u => u.Value._pair.Id() == instr.Id()).FirstOrDefault();
                if (subscribe.Value != null)
                {
                    try
                    {
                        subscribe.Value.Stop();
                        _subscibeList.Remove(subscribe.Key);
                        //_client.OnEvent(Name, BrokerEvent.CoinUnsubscribed, instr.Id());
                        _logger.Log(LogPriority.Info, $"Unsubscribed {instr.Symbol}", Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogPriority.Debug, $"Unsubscribe {instr.Id()} chnl fault", Name);
                        //_client.OnEvent(Name, BrokerEvent.CoinUnsubscribedFault, instr.Id());
                        _logger.Log(LogPriority.Info, $"Unsubscribed error {ex.Message}", Name);
                    }
                }
                else
                {
                    _logger.Log(LogPriority.Warning, $"Unsubscribe: {instr.Symbol} - is not subscribed", Name);
                    //_client.OnEvent(Name, BrokerEvent.CoinUnsubscribedFault, instr.Id());
                }
            }
        }

        #endregion
        public async Task<IEnumerable<Balance>> GetBalance()
        {
            //"accountAlias": "SgsR",    // unique account code
            //"asset": "USDT",    // asset name
            //"balance": "122607.35137903", // wallet balance
            //"crossWalletBalance": "23.72469206", // crossed wallet balance
            //"crossUnPnl": "0.00000000"  // unrealized profit of crossed positions
            //"availableBalance": "23.72469206",       // available balance
            //"maxWithdrawAmount": "23.72469206"
            string msg;
            try
            {
                msg = await _rest.SendRequest("/fapi/v2/balance", HttpMethod.Get, true);
                _logger.Log(LogPriority.BinanceWeight, $"GetBalance - Weight: 5", Name);
                var res = new BinanceResponseDto<List<BalanceDTO>>(msg);
                if (res.error is null)
                {
                    var result = res.data.Select(i => i.Convert()).ToList();
                    return result;
                } else
                {
                    _logger.Log(LogPriority.Error, $"<-- GetBalance error | ErrorCode: {res.error.Value.code}, ErrorMessege: {res.error.Value.msg}", Name);
                    return Enumerable.Empty<Balance>();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "Exception in BinanceApp.GetBalance");
                _logger.Log(LogPriority.Debug, ex, Name, "Exception in BinanceApp.GetBalance");
                throw new Exception("Failed to get balance. See inner exception for more details.", ex);
            }
        }
        public async Task<string> GetPositionMode()
        {
            try
            {
                var mode = await _rest.SendRequest("/fapi/v1/positionSide/dual", HttpMethod.Get, true);
                _logger.Log(LogPriority.BinanceWeight, $"GetPositionMode - Weight: 30", Name);
                var parse_mode = JObject.Parse(mode);
                if ((bool)parse_mode.GetValue("dualSidePosition"))
                {
                    return "Hedge Mode";
                }
                return "One-way Mode";
            }
            catch (Exception exc)
            {
                _logger.Log(LogPriority.Error, exc, Name, "Exception in BinanceApp.GetPositionMode");
                _logger.Log(LogPriority.Debug, exc, Name, "Exception in BinanceApp.GetPositionMode");
                throw new Exception("Failed to get position mode. See inner exception for more details.", exc);
            }
        }
        public async Task ChangePositionMode(bool value)
        {
            try
            {
                var param = $"dualSidePosition={value.ToString()}";
                var msg = await _rest.SendRequest("/fapi/v1/positionSide/dual", HttpMethod.Post, true, param);
                _logger.Log(LogPriority.BinanceWeight, $"ChangePositionMode - Weight: 1", Name);
                var parse_msg = JObject.Parse(msg);
                if (parse_msg.ContainsKey("code") && parse_msg.GetValue("msg").ToString() == "success")
                {
                    if (value)
                    {
                        _logger.Log(LogPriority.Info, $"Positions mode changed successfully to Hedge Mode", Name);
                    }
                    else
                    {
                        _logger.Log(LogPriority.Info, $"Positions mode changed successfully to One-way Mode", Name);
                    }
                }
                else
                {
                    _logger.Log(LogPriority.Info, $"Positions mode not changed! Reason: {parse_msg["msg"]}", Name);
                }
            }
            catch (Exception exc)
            {
                _logger.Log(LogPriority.Error, exc, Name, "Exception in BinanceApp.ChangePositionMode");
                _logger.Log(LogPriority.Debug, exc, Name, "Exception in BinanceApp.ChangePositionMode");
                throw new Exception("Failed to change position mode. See inner exception for more details.", exc);
            }
        }
        public async Task<Tuple<List<Position>, List<Balance>>> GetAccountInfo()
        {
            string msg;
            try
            {
                var positions = new List<Position>();
                var USDTBalance = new List<Balance>();
                msg = await _rest.SendRequest("/fapi/v2/account", HttpMethod.Get, true);
                _logger.Log(LogPriority.BinanceWeight, $"GetAccountInfo - Weight: 5", Name);
                var res = new BinanceResponseDto<AccountInfoDTO>(msg);
                if (res.error is null)
                {
                    positions = res.data.positions.Select(i => i.Convert()).ToList();
                    USDTBalance = res.data.assets.Select(i => i.ConvertToBalance()).ToList();
                } else
                {
                    _logger.Log(LogPriority.Error, $"<-- GetAccountInfo error | ErrorCode: {res.error.Value.code}, ErrorMessege: {res.error.Value.msg}", Name);
                }
                var result = new Tuple<List<Position>, List<Balance>>(positions, USDTBalance);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "Exception in BinanceApp.GetAccountInfo");
                _logger.Log(LogPriority.Debug, ex, Name, "Exception in BinanceApp.GetAccountInfo");
                throw new Exception("Failed to get account info. See inner exception for more details.", ex);
            }
        }

        public async Task<List<Position>> GetPosition(string symbol)
        {
            try
            {
                var result = new List<Position>();
                var param = $"symbol={symbol}";
                var msg = await _rest.SendRequest("/fapi/v2/positionRisk", HttpMethod.Get, true, param);
                _logger.Log(LogPriority.BinanceWeight, $"GetPosition - Weight: 5", Name);
                _logger.Log(LogPriority.Debug, $"--> REQUEST | GetPosition | Response: {msg}", Name);
                var res = new BinanceResponseDto<List<PositionDTO>>(msg);
                if (res.error is null)
                {
                    result = res.data.Select(i => i.Convert()).ToList();
                } else
                {
                    _logger.Log(LogPriority.Error, $"<-- Failed to get position {symbol} | ErrorCode: {res.error.Value.code}, ErrorMessege: {res.error.Value.msg}", Name);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "Exception in BinanceApp.GetPosition");
                _logger.Log(LogPriority.Debug, ex, Name, "Exception in BinanceApp.GetPosition");
                throw new Exception("Failed to get position. See inner exception for more details.", ex);
            }
        }
        public async Task<IEnumerable<Position>> GetPositions()
        {
            string msg;
            try
            {
                var result = new List<Position>();
                msg = await _rest.SendRequest("/fapi/v2/account", HttpMethod.Get, true);
                _logger.Log(LogPriority.BinanceWeight, $"GetPositions - Weight: 5", Name);
                var res = new BinanceResponseDto<AccountInfoDTO>(msg);
                if (res.error is null)
                {
                    result = res.data.positions.Select(i => i.Convert()).ToList();
                } else
                {
                    _logger.Log(LogPriority.Error, $"<-- Failed to get positions | ErrorCode: {res.error.Value.code}, ErrorMessege: {res.error.Value.msg}", Name);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "Exception in BinanceApp.GetPositions");
                _logger.Log(LogPriority.Debug, ex, Name, "Exception in BinanceApp.GetPositions");
                throw new Exception("Failed to get positions. See inner exception for more details.", ex);
            }
        }

        public async Task<MarketBook?> GetTicker(Instrument instrument)
        {
            try
            {
                var msg = await _rest.SendRequest($"/fapi/v1/ticker/bookTicker?symbol={Utilities.BinanceSymbol(instrument)}", HttpMethod.Get);
                _logger.Log(LogPriority.BinanceWeight, $"GetTicker - Weight:1 for a single symbol; 2 when the symbol parameter is omitted", Name);
                var res = new BinanceResponseDto<BookTickerDTO>(msg);
                if (res.error is null)
                {
                    var result = new MarketBook
                    {
                        Bid = Double.Parse(res.data.bidPrice),
                        Ask = Double.Parse(res.data.askPrice),
                        Time = Utilities.ConvertFromUnixTimestamp(res.data.time ?? default)
                    };
                    return result;
                }
                else
                {
                    _logger.Log(LogPriority.Error,
                        $"<-- Failed to get ticker | ErrorCode: {res.error.Value.code}, ErrorMessege: {res.error.Value.msg}",
                        Name);
                    throw new Exception("Failed to get ticker");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "Exception in BinanceApp.GetTicker");
                _logger.Log(LogPriority.Debug, ex, Name, "Exception in BinanceApp.GetTicker");
                throw new Exception("Failed to get ticker. See inner exception for more details.", ex);
            }
        }

        public async Task<DepthSnapshot> GetMarketBook(Instrument instr, int level)
        {
            var limit = Utilities.BinanceLvl(level).ToString();
            var msg = await _rest.SendRequest($"/fapi/v1/depth?symbol={Utilities.BinanceSymbol(instr)}&limit={limit}", HttpMethod.Get);
            _logger.Log(LogPriority.BinanceWeight, $"GetMarketBook - Weight: Adjusted based on the limit: limit = 5, 10, 20, 50 - Weight = 2; limit = 100 - Weight = 5; limit = 500 - Weight = 10; limit = 1000 - Weight = 20; Current limit = {limit}", Name);
            var res = new BinanceResponseDto<OrderBookDTO>(msg);
            if (res.error is null)
            {
                var lastId = res.data.lastUpdateId;
                var snapshotTime = res.data.T;
                var time = Utilities.ConvertFromUnixTimestamp(snapshotTime ?? default);
                var bid_list = res.data.bids.Select
                (j => new MarketUpdate
                {
                    Action = UpdateAction.New,
                    Type = EntryType.Bid,
                    Price = Double.Parse(j[0]),
                    Volume = Double.Parse(j[1])
                });
                var ask_list = res.data.asks.Select
                (j => new MarketUpdate
                {
                    Action = UpdateAction.New,
                    Type = EntryType.Offer,
                    Price = Double.Parse(j[0]),
                    Volume = Double.Parse(j[1])
                });
                return new DepthSnapshot(bid_list, ask_list, time, lastId ?? default);
            }
            else
            {
                _logger.Log(LogPriority.Error,
                    $"<-- Failed to get market book | ErrorCode: {res.error.Value.code}, ErrorMessege: {res.error.Value.msg}",
                    Name);
                throw new Exception("Failed to get market book");
            }
        }
        public async Task<long?> SetLeverage(Instrument instr, uint leverage)
        {
            try
            {
                if (leverage > 125)
                {
                    throw new Exception($"Try set incorrect leverage - {leverage}. Max leverage is 125");
                }
                var msg = await _rest.SendRequest("/fapi/v1/leverage", HttpMethod.Post, true, $"symbol={Utilities.BinanceSymbol(instr)}&leverage={leverage}");
                _logger.Log(LogPriority.BinanceWeight, $"SetLeverage - Weight: 1 ", Name);
                var res = new BinanceResponseDto<LeverageDTO>(msg);
                if (res.error is null)
                {
                    _logger.Log(LogPriority.Debug, $"Set leverage for {res.data.symbol} = {res.data.leverage}", Name);
                    return res.data.leverage;
                } else
                {
                    _logger.Log(LogPriority.Error, $"<-- Failed to set leverage | ErrorCode: {res.error.Value.code}, ErrorMessege: {res.error.Value.msg}", Name);
                    throw new Exception("Failed to set leverage");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "Exception in BinanceApp.SetLeverage");
                _logger.Log(LogPriority.Debug, ex, Name, "Exception in BinanceApp.SetLeverage");
                throw new Exception("Failed to set leverage. See inner exception for more details.", ex);
            }
        }

        public async Task<int> GetLeverage(Instrument instr)
        {
            try
            {
                var msg = await _rest.SendRequest("/fapi/v2/account", HttpMethod.Get, true);
                _logger.Log(LogPriority.BinanceWeight, $"GetLeverage - Weight: 5 ", Name);
                var res = new BinanceResponseDto<AccountInfoDTO>(msg);
                if (res.error is null)
                {
                    var leverage = Int32.Parse(res.data.positions.FirstOrDefault(i => i.symbol == instr.Symbol).leverage);
                    _logger.Log(LogPriority.Debug, $"Get leverage value for {instr.Symbol} = {leverage}", Name);
                    return leverage;
                } else
                {
                    _logger.Log(LogPriority.Error, $"<-- Failed to get leverage | ErrorCode: {res.error.Value.code}, ErrorMessege: {res.error.Value.msg}", Name);
                    throw new Exception("Failed to get leverage");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "Exception in BinanceApp.GetLeverage");
                _logger.Log(LogPriority.Debug, ex, Name, "Exception in BinanceApp.GetLeverage");
                throw new Exception("Failed to get leverage. See inner exception for more details.", ex);
            }
        }

        public async Task<MarginType> ChangeMarginType(Instrument instr, MarginType marginType)
        {
            await _rest.SendRequest("/fapi/v1/marginType", HttpMethod.Post, true, $"symbol={Utilities.BinanceSymbol(instr)}&marginType={marginType.ToString().ToUpper()}");
            _logger.Log(LogPriority.BinanceWeight, $"ChangeMarginType - Weight: 1 ", Name);

            return marginType;
        }
    }
}
