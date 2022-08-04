using System;
using System.Linq;
using System.Threading.Tasks;
using BinanceFrontEnd;
using Microsoft.Extensions.Configuration;
using Shared.Interfaces;
using WebRouterApp.Core.Data;
using WebRouterApp.Core.Infrastructure.ConfigurationParts;
using WebRouterApp.Features.Publishers.Application.Models;
using WebRouterApp.Features.Trading.SignalR;
using WebRouterApp.Shared.Services;

namespace WebRouterApp.Core.CopyEngineParts
{
    public class CopyEngine
    {
        private readonly IConfiguration _configuration;
        private readonly ServiceScopeFactory<TradeRouterDbContext> _dbContext;
        private readonly ILogger _log;
        private readonly TradingMessageQueue _tradingMessageQueue;

        // We plan to eventually create a session per user.
        // But at this stage, our app is effectively single-user.
        private CopySession? _copySession;

        public CopyEngine(
            IConfiguration configuration,
            ILogger log,
            TradingMessageQueue tradingMessageQueue,
            ServiceScopeFactory<TradeRouterDbContext> dbContext)
        {
            _configuration = configuration;
            _log = log;
            _tradingMessageQueue = tradingMessageQueue;
            _dbContext = dbContext;
        }

        public async Task EnsureSessionCreated(Guid userId)
        {
            if (_copySession != null)
                return;

            var publisher = CreatePublisher();

            using (var dbContext = _dbContext.BeginRequired())
            {
                foreach (var subscriberRecord in dbContext.Value.Subscribers)
                    publisher.Add(subscriberRecord);
            }

            _copySession = new CopySession(publisher);
            await _copySession.Publisher.Start();
        }

        private Publisher CreatePublisher()
        {
            var settings = AppSettingConstants.PublisherKeys
                .Select(key => (key: key, value: _configuration.GetAppSetting(key)))
                .ToList();

            PublisherRecord record;
            using (var dbContext = _dbContext.BeginRequired())
            {
                record = dbContext.Value.Publishers.First();
            }

            settings.AddRange(new[]
            {
                (key: AppSettingConstants.ApiKey, value: record.ApiKey),
                (key: AppSettingConstants.ApiSecret, value: record.ApiSecret)
            });

            var publisher = new Publisher(
                record,
                _log,
                new BinanceFront(_log, record.Name, settings),
                _tradingMessageQueue,
                _configuration);

            return publisher;
        }

        public CopySession? GetSession(Guid userId)
        {
            // We plan to eventually create a session per user.
            // But at this stage, our app is effectively single-user.
            return _copySession;
        }
    }
}
