using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceFuturesConnector;
using Shared.Broker;
using Shared.Interfaces;
using Shared.Models;
using SharedBinance.Broker;
using SharedBinance.Models;
using Utility.ExecutionContext;
using Utf8Json;
using Utf8Json.Resolvers;

namespace ConsoleTest
{
    public sealed class DefaultDataStorage : IDataStorage
    {

        private ImmutableDictionary<string, string> _localStorage;

        public DefaultDataStorage(IEnumerable<string> keys = null)
            => _localStorage = (keys ?? Enumerable.Empty<string>()).ToImmutableDictionary(i => i, _ => null as string);

        public string Get(string key)
            => ImmutableInterlocked.GetOrAdd(ref _localStorage, key, _ => null);

        public bool Set(string key, string value)
            => ImmutableInterlocked.TryUpdate(ref _localStorage, key, value, null);

        public string[] AvailableKeys => _localStorage.Keys.ToArray();

    }
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            CompositeResolver.RegisterAndSetAsDefault(StandardResolver.AllowPrivateExcludeNull);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            
            IDataStorage sett = new DefaultDataStorage(new string []{"APIKey", "APISecret", "Url", "Wss"});
            sett.Set("APIKey", ConfigurationManager.AppSettings["APIKey"]);
            sett.Set("APISecret", ConfigurationManager.AppSettings["APISecret"]);
            sett.Set("Url", ConfigurationManager.AppSettings["Url"]);
            sett.Set("Wss", ConfigurationManager.AppSettings["Wss"]);
            ILogger globalLogger = new ConsoleLogger();
            Worker context = new Worker(globalLogger);

            context.Start();

            await Task.Delay(5000);

            // context.Put(async _ =>
            // {
            //     Console.WriteLine("XXXXXXXXXXX");
            // });
            // context.Put(async _ =>
            // {
            //     Console.WriteLine("^^^^");
            // });
            // context.Put(async _ =>
            // {
            //     Console.WriteLine("ASFASFASF");
            // });
            //
            // context.Stop();
            //
            //
            // context.Put(async _ =>
            // {
            //     Console.WriteLine("32333");
            // });
            // context.Put(async _ =>
            // {
            //     Console.WriteLine("ASDFAFDSAF");
            // });
            // context.Put(async _ =>
            // {
            //     Console.WriteLine("!!!");
            // });
            //
            // Console.ReadLine();

             IBrokerClient client = new BinanceClient("Master test", globalLogger, sett);
             var result = client.StartAsync();
             Order order = new Order();
             Instrument instr = new Instrument
                 { Symbol = "ETHUSDT", Exchange = "Binance" };
             Instrument instr2 = new Instrument
                 { Symbol = "ETHUSDT", Exchange = "Binance" };
             Console.WriteLine($"Error - {log4net.Core.Level.Error.Value}, Warn - {log4net.Core.Level.Warn.Value}" +
                 $" Info - {log4net.Core.Level.Info.Value}, Debug - {log4net.Core.Level.Debug.Value}, Notice - {log4net.Core.Level.Notice.Value}");
             while (true)
             {
                 var enter = Console.ReadLine();
                 if (enter == "bk")
                 {
                     try
                     {
                         var res = await client.GetMarketBook(instr, 20);
            
                         foreach (var i in res.asks)
                         {
                             Console.WriteLine($"ask/{i.Price}{i.Volume}");
                         }
                         foreach (var i in res.bids)
                         {
                             Console.WriteLine($"bid/{i.Price}{i.Volume}");
                         }
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine($"Balance exception = {e.Message}");
                     }
                 }
                 if (enter == "t")
                 {
                     try
                     {
                         var ticker = await client.GetTicker(instr);
                         Console.WriteLine($"Ticker - {ticker.Value.Bid}/{ticker.Value.Ask}");
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine($"Cancel exception = {e.Message}");
                     }
                 }
                 if (enter == "l")
                 {
                     try
                     {
                         var leverage = await client.SetLeverage(instr, 10);
                         Console.WriteLine($"Leverage - {leverage}");
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine($"Cancel exception = {e.Message}");
                     }
                 }
                 if (enter == "ca")
                 {
                     try
                     {
                         var instruments = await client.GetInstruments(); ;
                         var tasks = instruments.Select(i => client.CancelAllOrders(i.Symbol)).ToArray();
                         foreach (var i in tasks)
                         {
                             await i;
                         }
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine($"Cancel exception = {e.Message}");
                     }
                 }
                 if (enter == "gl")
                 {
                     try
                     {
                         var leverage = await client.GetLeverage(instr);
                         Console.WriteLine($"Leverage - {leverage}");
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine($"Cancel exception = {e.Message}");
                     }
                 }
                 if (enter == "m")
                 {
                     try
                     {
                         var marginType = await client.ChangeMarginType(instr, MarginType.Crossed);
                         Console.WriteLine($"MarginType - {marginType}");
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine($"Cancel exception = {e.Message}");
                     }
                 }
                 if (enter == "gall")
                 {
                     try
                     {
                         var list = await client.GetAllOpenOrderds();
                         if (list.Count > 0)
                         {
                             foreach (var ord in list)
                             {
                                 Console.WriteLine($"symbol - {ord.Symbol}, price - {ord.Price.ToString()}, ID - {ord.ClientId}, Side - {ord.OrderSide}, Position - {ord.PositionSide}");
                             }
                         }
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine($"{e.Message}");
                     }
                 }
                 if (enter == "b")
                 {
                     try
                     {
                         var balance = await client.GetBalance();
                         foreach (var item in balance)
                         {
                             if (item.Value > 0)
                             {
                                 Console.WriteLine($"Balance - {item.Currency} - {item.Value}");
                             }
                         }
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine($"Balance exception = {e.Message}");
                     }
                 }
                 else if (enter == "i")
                 {
                     var Instruments = await client.GetInstruments();
                     var rsrusdt = Instruments.First(i => i.Symbol == "LTCUSDT".ToUpper()).QuantityPrecision;
                     Console.WriteLine(Instruments.First(i => i.Symbol == "LTCUSDT".ToUpper()).QuantityPrecision);
                     foreach (var i in Instruments)
                     {
            
                         Console.WriteLine($"{i.Symbol}");
                     }
                 }
                 else if (enter == "s")
                 {
                     await client.Subscribe(instr, SubscriptionModel.TopBook);
                     client.MarketDataHandler += (_, symbol, book, depth, updates) =>
                     {
                         Console.WriteLine($"Symbol: {symbol}, Asks: {book.Ask}, Bids: {book.Bid}");
                     };
                 }
                 else if (enter == "u")
                 {
                     await client.Unsibscribe(instr);
                 }
                 else if (enter == "test")
                 {
                     order = new Order();
                     instr = new Instrument
                         { Symbol = "BTCUSDT", Exchange = "Binance" };
                     order.Symbol = instr.Symbol;
                     order.Amount = 0.031;
                     order.Price = 37511;
                     order.OrderSide = OrderSide.BUY;
                     order.OrderType = OrderType.LIMIT;
                     order.PositionSide = PositionSide.LONG;
                     order.ClientId = "19646139055";
                     await client.SendNewOrder(order);
                 }
                 else if (enter == "ol")
                 {
                     order = new Order();
                     instr = new Instrument
                         { Symbol = "ETHUSDT", Exchange = "Binance" };
            
                     order.Symbol = instr.Symbol;
                     order.Amount = 0.003;
                     order.Price = 1800;
                     order.OrderSide = OrderSide.BUY;
                     order.OrderType = OrderType.MARKET;
                     order.PositionSide = PositionSide.LONG;
                     order.ClientId= "123456789";
                     await client.SendNewOrder(order);
                 }
                 else if (enter == "os")
                 {
                     order = new Order();
                     instr = new Instrument
                         { Symbol = "ETHUSDT", Exchange = "Binance" };
            
                     order.Symbol = instr.Symbol;
                     order.Amount = 0.003;
                     order.Price = 1800;
                     order.OrderSide = OrderSide.SELL;
                     order.OrderType = OrderType.MARKET;
                     order.PositionSide = PositionSide.SHORT;
                     await client.SendNewOrder(order);
                 }
            
                 else if (enter == "cl")
                 {
                     order = new Order();
                     instr = new Instrument
                         { Symbol = "ETHUSDT", Exchange = "Binance" };
            
                     order.Symbol = instr.Symbol;
                     order.Amount = 0.003;
                     //order.Price = 2900;
                     order.OrderSide = OrderSide.SELL;
                     order.OrderType = OrderType.MARKET;
                     order.PositionSide = PositionSide.LONG;
                     //order.StopPrice = 4900;
                     //order.ClientId = "12345678090";
                     //order.ClosePosition = true;
                     //order.ClientOrderId = "test";
                     await client.SendNewOrder(order);
                 }
                 else if (enter == "cs")
                 {
                     order = new Order();
                     instr = new Instrument
                         { Symbol = "ETHUSDT", Exchange = "Binance" };
            
                     order.Symbol = instr.Symbol;
                     order.Amount = 0.003;
                     //order.Price = 2300;
                     order.OrderSide = OrderSide.BUY;
                     order.OrderType = OrderType.MARKET;
                     order.PositionSide = PositionSide.SHORT;
                     //order.ClientOrderId = "test";
                     await client.SendNewOrder(order);
                 }
            
            
                 else if (enter == "c")
                 {
                     // instr = new Instrument() { Symbol = "LTCUSDT", Exchange = "Binance" };
                     // instr = new Instrument() {Symbol = "ETHUSDT_210625", Exchange = "Binance" };
                     order.ExchangeId = "11314643046";
            
                     //order.Symbol = instr.Symbol;
                     //var tempId = order.Id;
                     order.ClientId= "123456789";
                     order.Symbol = "ETHUSDT";
                     //Debug | Broker(Binance) new order '2c53a8ce-3782-473a-aaad-49c160381167'
                     //await connector.Broker.CancelOrder(order);
                     //order.Id = tempId;
                     await client.CancelOrder(order);
                 }
                 else if (enter == "cao")
                 {
                     await client.CancelAllOrders("ETHUSDT");
                 }
                 else if (enter == "pos")
                 {
                     instr = new Instrument() { Symbol = "LTCUSDT", Exchange = "Binance" };
                     instr = new Instrument() {Symbol = "ETHUSDT_210625", Exchange = "Binance" };
                     order.ExchangeId = "11314643046";
            
                     order.Symbol = instr.Symbol;
                     order.ClientId = "";
                     var positions = (await client.GetPositions()).Where(i => i.positionAmt != 0);
                     foreach (var i in positions)
                     {
            
                         Console.WriteLine($"entryPrice = {i.entryPrice},positionAmt = {i.positionAmt},positionSide = {i.positionSide},symbol = {i.symbol}," +
                             $"marginType = {i.marginType},isAutoAddMargin = {i.isAutoAddMargin},isolatedMargin = {i.isolatedMargin},markPrice = {i.markPrice}" +
                             $"unRealizedProfit = {i.unRealizedProfit},maxNotionalValue = {i.maxNotionalValue},leverage = {i.leverage}");
                     }
                 }
                 else if (enter == "position")
                 {
                     //instr = new Instrument() { Symbol = "LTCUSDT", Exchange = "Binance" };
                     //instr = new Instrument() {Symbol = "ETHUSDT_210625", Exchange = "Binance" };
                     //order.ExchId = "11314643046";
            
                     //order.Symbol = instr.Symbol;
                     //order.Id = "";
                     //Debug | Broker(Binance) new order '2c53a8ce-3782-473a-aaad-49c160381167'
                     var instrument = new Instrument
                         { Symbol = "ETHUSDT", Exchange = "Binance" };
                     var i = await client.GetPosition(instrument.Symbol);
            
                 }
                 else if (enter == "gmod")
                 {
                     var posMode = await client.GetPositionMode();
                     Console.WriteLine($"Mod: {posMode}");
                 }
                 else if (enter == "chmod")
                 {
                     await client.ChangePositionMode(false);
                 }
             }
        }
    }
}
