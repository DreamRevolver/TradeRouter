
using System;
using System.Collections.Generic;

using Shared.Broker;
using Shared.Models;

using SharedBinance.Models;
namespace Shared.Interfaces
{
    public abstract class FrontEndClient : IEndpoint
    {
        public abstract event Action<ExecutionReport> OrderUpdateEvent;
        public abstract event Action<ConnectionEvent> ConnectionEvent;
        public abstract event Action<TradingSignal> SignalEvent;
        public abstract event Action<Position> PositionChanged;
        public abstract event Action<MarketBook, string> MarketDataUpdateEvent;
        public abstract event Action<(string, int)> LeverageChanged;
        public abstract void Start();
        public abstract void Stop();
        public abstract bool IsRunning { get; }
        public IDataStorage Settings { get; }
        public abstract IBrokerClient Connector { get; }
        public abstract List<Order> MasterOrders { get; }
        public abstract List<Position> MasterPositions { get; }
        public FrontEndClient()
            => Settings = new SettingsStorage(this);
    }
}
