using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;
using SharedBinance.Models;
namespace Shared.Broker
{
    /// <summary>
    /// Represent adapter internal events.
    /// </summary>
    public enum BrokerEvent
    {
        ConnectorStarted,
        ConnectorStopped,
        SessionLogon,
        SessionLogout,
        InternalError,
        ParseError,
        Info,
       // Debug,
        //Ready,
        CoinSubscribed,
        CoinUnsubscribed,
        CoinSubscribedFault,
        CoinUnsubscribedFault,
        GatewayError
    }
    public interface IBrokerClient
    {
        event Action<string, BrokerEvent, string> BrokerEventHandler;
        event Action<string, ExecutionReport> ExecutionReportHandler;
        event Action<(string, int)> AccountConfigUpdateHandler;
        event Action<string, List<Position>> PositionUpdateHandler;
        event Action<string, List<Balance>> BalanceUpdateHandler;
        event Action<string, Order> OrderHandler;
        event Action<string, string, MarketBook, DepthSnapshot, IEnumerable<MarketUpdate>> MarketDataHandler;
        Task<IEnumerable<Balance>> GetBalance();
        Task<List<Position>> GetPosition(string instr);
        Task<IEnumerable<Position>> GetPositions();
        Task<MarketBook?> GetTicker(Instrument instr);
        Task<DepthSnapshot> GetMarketBook(Instrument instr, int level);
        Task<long?> SetLeverage(Instrument instr, uint leverage);
        Task<int> GetLeverage(Instrument instr);
        Task<MarginType> ChangeMarginType(Instrument instr, MarginType marginType);
        Task Subscribe(Instrument instr, SubscriptionModel model);
        Task Unsibscribe(Instrument instr);
        Task CancelAllOrders(string symbol);
        Task<List<Instrument>> GetInstruments();
        Task<Order> SendNewOrder(Order order);
        Task<Order> CancelOrder(Order order);
        Task<Order> ModifyOrder(Order order, double price, double stopPrice, uint qty);
        string Name { get; }
        Task<List<Order>> GetAllOpenOrderds();
        Task<string> GetPositionMode();
        Task ChangePositionMode(bool value);
        bool? IsConnected { get; }
        Task<Tuple<List<Position>, List<Balance>>> GetAccountInfo();
        Task StartAsync();
        void Stop();
    }

    public enum SubscriptionModel
    {
        TopBook,
        FullBook,
        Incremental
    }
}
