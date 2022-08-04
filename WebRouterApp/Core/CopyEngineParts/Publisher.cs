using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BinanceBackEnd;
using Microsoft.Extensions.Configuration;
using Shared.Interfaces;
using Shared.Models;
using Utility.ExecutionContext;
using WebRouterApp.Core.CopyEngineParts.TraderParts;
using WebRouterApp.Core.Infrastructure.ConfigurationParts;
using WebRouterApp.Features.Publishers.Application.Models;
using WebRouterApp.Features.Subscribers.Application.Models;
using WebRouterApp.Features.Trading.SignalR;

namespace WebRouterApp.Core.CopyEngineParts
{
    public class Publisher : Trader
    {
        private readonly IConfiguration _configuration;
        private readonly List<Subscriber> _subscribers;

        public Publisher(
            PublisherRecord record,
            ILogger log,
            FrontEndClient frontEndClient,
            TradingMessageQueue tradingMessageQueue,
            IConfiguration configuration)
            : base("Publisher", log, ExchangeClient.From(frontEndClient), tradingMessageQueue)
        {
            _subscribers = new List<Subscriber>();

            Record = record;
            FrontEndClient = frontEndClient;
            _configuration = configuration;
            FrontEndClient.SignalEvent += OnSignal;
            FrontEndClient.LeverageChanged += OnLeverageChanged;
        }

        public override Guid Id { get; } = Guid.Parse("95696A4D-0D23-438A-BBD2-0E3CE7C460EF");

        public PublisherRecord Record { get; }
        public FrontEndClient FrontEndClient { get; }
        public IReadOnlyList<Subscriber> Subscribers => _subscribers.ToList();
        public IEnumerable<Subscriber> RunningSubscribers => _subscribers.Where(it => it.IsRunning);

        public string FormatBalance(WalletCurrency currency)
        {
            return "--";
        }

        public void Add(SubscriberRecord record)
        {
            var subscriber = CreateSubscriber(record);
            _subscribers.Add(subscriber);
        }

        public void Update(SubscriberRecord record)
        {
            Remove(record.Id);
            Add(record);
        }

        public void Remove(Guid subscriberId)
        {
            _subscribers.RemoveAll(it => it.Id == subscriberId);
        }

        private Subscriber CreateSubscriber(SubscriberRecord record)
        {
            var taskQueue = new Worker(Log);

            var settings = AppSettingConstants.SubscriberKeys
                .Select(key => (key: key, value: _configuration.GetAppSetting(key)))
                .ToList();

            settings.AddRange(new[]
            {
                (key: "APIKey", value: record.ApiKey),
                (key: "APISecret", value: record.ApiSecret),
                (key: "Mode", value: "Amt Multiplier"),
                (key: "ModeValue", value: record.Multiplier.ToString(CultureInfo.InvariantCulture))
            });

            var exchangeClient = new BinanceBack(
                taskQueue,
                Log,
                record.Name,
                settings,
                FrontEndClient);

            var subscriber = new Subscriber(
                record,
                Log,
                exchangeClient,
                TradingMessageQueue);

            return subscriber;
        }

        private void OnLeverageChanged((string, int) changeInfo)
        {
            foreach (var runningSubscriber in RunningSubscribers)
                runningSubscriber.ChangeLeverage(changeInfo);
        }

        private void OnSignal(TradingSignal signal)
        {
            foreach (var runningSubscriber in RunningSubscribers)
                runningSubscriber.PushSignal(signal);
        }
    }
}
