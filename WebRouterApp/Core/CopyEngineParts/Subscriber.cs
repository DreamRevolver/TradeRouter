using System;
using System.Threading.Tasks;
using Shared.Core;
using Shared.Interfaces;
using Shared.Models;
using WebRouterApp.Core.CopyEngineParts.TraderParts;
using WebRouterApp.Features.Subscribers.Application.Models;
using WebRouterApp.Features.Trading.SignalR;

namespace WebRouterApp.Core.CopyEngineParts
{
    public class Subscriber : Trader
    {
        private readonly BackEndClient _backEndClient;
        
        public Subscriber(
            SubscriberRecord record, 
            ILogger log, 
            BackEndClient backEndClient, 
            TradingMessageQueue tradingMessageQueue)
            : base(record.Name, log, ExchangeClient.From(backEndClient), tradingMessageQueue)
        {
            _backEndClient = backEndClient;
            Record = record;
        }

        public override Guid Id => Record.Id;
        public SubscriberRecord Record { get; }
        
        public void PushSignal(TradingSignal signal) => _backEndClient.PushSignal(signal);

        public void ChangeLeverage((string, int) changeInfo) => _backEndClient.ChangeLeverage(changeInfo);
        public string FormatBalance(WalletCurrency currency) => _backEndClient.GetBalanceValue(currency);

        public Task CancelAllOrders() => _backEndClient.CancelAllOrders();
        public Task CloseAllPositions() => _backEndClient.CloseAllPositions();
        public Task SynchronizeOrders() => _backEndClient.SynchronizeOrders();
        public Task SynchronizePositions() => _backEndClient.SynchronizePositions();

    }
}
