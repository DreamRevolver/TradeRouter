
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Broker;
using Shared.Interfaces;
using Shared.Models;

using SharedBinance.Models;
namespace Shared.Core
{
    public abstract class BackEndClient : IEndpoint
    {
        public abstract event Action<ExecutionReport, string> OrderUpdateEvent;
        public abstract event Action<string, List<Balance>, string> BalanceChangeEvent;
        public abstract event Action<ConnectionEvent> ConnectionEvent;
        public abstract event Action<Position> PositionChanged;
        public abstract event Action<MarketBook, string> MarketDataUpdateEvent;
        public abstract void PushSignal(TradingSignal signal);
        public abstract Task Start();
        public abstract void Stop();
        public abstract string Name { get; }
        public abstract Task CancelAllOrders();
        public abstract Task CloseAllPositions();
        public abstract Task SynchronizeOrders();
        public abstract Task SynchronizePositions();
        public abstract string GetBalanceValue(WalletCurrency currency);
        public abstract bool IsRunning { get;}
        public IDataStorage Settings { get; }
        public BackEndClient()
            => Settings = new SettingsStorage(this);

        public abstract IBrokerClient Connector { get; }
        public abstract bool AllowChangeSettings();
        public abstract void ChangeLeverage((string, int) param);
    }
}
