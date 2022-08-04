using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Shared.Broker;
using Shared.Interfaces;
using Shared.Models;
using SharedBinance.Models;
using WebRouterApp.Features.Trading.Application.MessageParts;
using WebRouterApp.Features.Trading.SignalR;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Core.CopyEngineParts.TraderParts
{
    public abstract class Trader
    {
        private readonly AsyncLock _isRunningLock = new();
        private readonly string _name;

        protected readonly ExchangeClient ExchangeClient;

        protected readonly ILogger Log;
        protected readonly TradingMessageQueue TradingMessageQueue;

        private OrderSet _openOrders;
        private PositionSet _positions;

        protected Trader(
            string name,
            ILogger log,
            ExchangeClient exchangeClient,
            TradingMessageQueue tradingMessageQueue)
        {
            _openOrders = OrderSet.Empty;
            _positions = PositionSet.Empty;

            _name = name;
            Log = log;
            ExchangeClient = exchangeClient;
            TradingMessageQueue = tradingMessageQueue;

            ExchangeClient.ConnectionEvent += OnConnectionStateChanged;
            ExchangeClient.OrderUpdateEvent += OnOrderUpdated;
            ExchangeClient.MarketDataUpdateEvent += OnMarketDataUpdated;
            ExchangeClient.PositionChanged += OnPositionChanged;
        }

        public abstract Guid Id { get; }
        public bool IsRunning => ExchangeClient.IsRunning;

        public DateTime? UtcStartedAt { get; private set; }

        public IReadOnlyList<Order> Orders => _openOrders.Values;
        public IReadOnlyList<Position> Positions => _positions.Values;

        public async Task Start()
        {
            if (IsRunning)
                return;

            using (await _isRunningLock.LockAsync())
            {
                if (IsRunning)
                    return;

                UtcStartedAt = DateTime.UtcNow;

                await Try(() => ExchangeClient.Start());
            }

            await Try(async () =>
            {
                _openOrders = OrderSet.With(await ExchangeClient.Broker.GetAllOpenOrderds());
                _ = TradingMessageQueue.Enqueue(Messages.OrdersSnapshot(Id, _openOrders.Values));
            });

            await Try(async () =>
            {
                var allPositions = await ExchangeClient.Broker.GetPositions() ?? new List<Position>();
                _positions = PositionSet.With(allPositions.Where(it => it.positionAmt != 0));
                _ = TradingMessageQueue.Enqueue(Messages.PositionsSnapshot(Id, _positions.Values));

                foreach (var position in _positions.Values)
                    await ExchangeClient.Broker.Subscribe(
                        new Instrument { Symbol = position.symbol },
                        SubscriptionModel.TopBook);
            });
        }

        public async Task Stop()
        {
            if (!IsRunning)
                return;

            using (await _isRunningLock.LockAsync())
            {
                if (!IsRunning)
                    return;

                UtcStartedAt = null;

                await ExchangeClient.Stop();
            }
        }

        private void OnConnectionStateChanged(ConnectionEvent @event)
        {
            Log.Log(LogPriority.Info, @event.ToString(), _name);
        }

        private void OnOrderUpdated(ExecutionReport report)
        {
            var addedOrders = new List<Order>();
            var removedOrders = new List<Order>();

            var changedOrder = report.ToOrder();
            var resultCode = _openOrders.Apply(changedOrder);

            if (resultCode == OrderSet.ApplyResultCode.OrderAdded)
                addedOrders.Add(changedOrder);
            else if (resultCode == OrderSet.ApplyResultCode.OrderRemoved) removedOrders.Add(changedOrder);

            PostMessage(
                Messages.OrdersChanged(
                    Id,
                    addedOrders,
                    removedOrders));
        }

        private void OnPositionChanged(List<Position> newOrChangedPositions)
        {
            var addedPositions = new List<Position>();
            var updatedPositions = new List<Position>();
            var removedPositions = new List<Position>();

            foreach (var newOrChangedPosition in newOrChangedPositions)
            {
                var resultCode = _positions.ApplyChanged(newOrChangedPosition);

                if (resultCode == PositionSet.ApplyChangedResultCode.AddedPosition)
                {
                    addedPositions.Add(newOrChangedPosition);

                    // If it's a new position, we'd like to get updated market data for the relevant symbol.
                    ExchangeClient.Broker.Subscribe(
                        new Instrument { Symbol = newOrChangedPosition.symbol },
                        SubscriptionModel.TopBook);

                    continue;
                }

                if (resultCode == PositionSet.ApplyChangedResultCode.RemovedPosition)
                {
                    removedPositions.Add(newOrChangedPosition);

                    if (!_positions.AnyWithSymbol(newOrChangedPosition.symbol))
                        ExchangeClient.Broker.Unsibscribe(
                            new Instrument { Symbol = newOrChangedPosition.symbol }
                        );

                    continue;
                }

                updatedPositions.Add(newOrChangedPosition);
            }

            PostMessage(
                Messages.PositionsChanged(
                    Id,
                    addedPositions,
                    updatedPositions,
                    removedPositions));
        }

        private void OnMarketDataUpdated(MarketBook marketBook, string symbol)
        {
            var updatedPositions = new List<Position>();

            var longPositions = _positions.LongWithSymbol(symbol).ToReadOnlyList();
            foreach (var it in longPositions)
                it.unRealizedProfit = (marketBook.Bid - it.entryPrice) * it.positionAmt;

            updatedPositions.AddRange(longPositions);

            var shortPositions = _positions.ShortWithSymbol(symbol).ToReadOnlyList();
            foreach (var it in shortPositions)
                it.unRealizedProfit = (it.entryPrice - marketBook.Ask) * it.positionAmt;

            updatedPositions.AddRange(shortPositions);

            PostMessage(Messages.PositionsChanged(Id, updated: updatedPositions));
        }

        private void PostMessage(OrdersChangedMessage? message)
        {
            if (message == null)
                return;

            _ = TradingMessageQueue.Enqueue(message);
        }

        private void PostMessage(PositionsChangedMessage? message)
        {
            if (message == null)
                return;

            _ = TradingMessageQueue.Enqueue(message);
        }

        private async Task Try(Func<Task> asyncAction)
        {
            try
            {
                await asyncAction.Invoke();
            }
            catch (Exception ex)
            {
                Log.Error(ex, source: _name, ex.ToString());
            }
        }
    }
}
