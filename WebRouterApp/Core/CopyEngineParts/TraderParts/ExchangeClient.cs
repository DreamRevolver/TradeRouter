using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Broker;
using Shared.Core;
using Shared.Interfaces;
using Shared.Models;
using SharedBinance.Models;

namespace WebRouterApp.Core.CopyEngineParts.TraderParts
{
    public class ExchangeClient
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly object _managedClient;
        
        private readonly Func<Task> _startExchangeClient;
        private readonly Func<Task> _stopExchangeClient;
        private readonly Func<bool> _isExchangeClientRunning;

        public event Action<ConnectionEvent> ConnectionEvent = delegate { };
        public event Action<ExecutionReport> OrderUpdateEvent = delegate { };
        public event Action<List<Position>> PositionChanged = delegate { };
        public event Action<MarketBook, string> MarketDataUpdateEvent = delegate { };

        private ExchangeClient(
            object managedClient, 
            IBrokerClient broker,
            Func<Task> startExchangeClient, 
            Func<Task> stopExchangeClient, 
            Func<bool> isExchangeClientRunning)
        {
            _managedClient = managedClient;
            Broker = broker;
            _startExchangeClient = startExchangeClient;
            _stopExchangeClient = stopExchangeClient;
            _isExchangeClientRunning = isExchangeClientRunning;
        }

        public bool IsRunning => _isExchangeClientRunning();
        public IBrokerClient Broker { get; }

        public Task Start() => _startExchangeClient();
        public Task Stop() => _stopExchangeClient();

        public static ExchangeClient From(BackEndClient backendClient)
        {
            var exchangeClient = new ExchangeClient(
                backendClient,
                backendClient.Connector,
                startExchangeClient: () => backendClient.Start(),
                stopExchangeClient: () => { backendClient.Stop(); return Task.CompletedTask; },
                isExchangeClientRunning: () => backendClient.IsRunning
            );

            backendClient.ConnectionEvent += @event => exchangeClient.ConnectionEvent(@event);
            backendClient.OrderUpdateEvent += (report, _) => exchangeClient.OrderUpdateEvent(report);
            backendClient.PositionChanged += changedPositions => exchangeClient.PositionChanged(new() {changedPositions});
            backendClient.MarketDataUpdateEvent += (snapshot, symbol) => exchangeClient.MarketDataUpdateEvent(snapshot, symbol);
            
            return exchangeClient;
        }

        public static ExchangeClient From(FrontEndClient frontendClient)
        {
            var exchangeClient = new ExchangeClient(
                frontendClient,
                frontendClient.Connector,
                startExchangeClient: () => { frontendClient.Start(); return Task.CompletedTask; },
                stopExchangeClient: () => { frontendClient.Stop(); return Task.CompletedTask; },
                isExchangeClientRunning: () => frontendClient.IsRunning
            );

            frontendClient.ConnectionEvent += @event => exchangeClient.ConnectionEvent(@event);
            frontendClient.OrderUpdateEvent += report => exchangeClient.OrderUpdateEvent(report);
            frontendClient.PositionChanged += changedPositions => exchangeClient.PositionChanged(new() {changedPositions});
            frontendClient.MarketDataUpdateEvent += (snapshot, symbol) => exchangeClient.MarketDataUpdateEvent(snapshot, symbol);
            
            return exchangeClient;
        }
    }
}
