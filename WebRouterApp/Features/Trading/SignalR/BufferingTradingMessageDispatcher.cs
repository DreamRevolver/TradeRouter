using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using WebRouterApp.Features.Trading.Application.MessageParts;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Features.Trading.SignalR
{
    public class BufferingTradingMessageDispatcher : BackgroundService
    {
        private static readonly TimeSpan BufferingPeriod = TimeSpan.FromMilliseconds(500);

        private readonly AsyncLock _bufferLocker = new();
        private readonly ILogger<BufferingTradingMessageDispatcher> _log;
        private readonly TradingMessageQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private Buffer _buffer = new();

        public BufferingTradingMessageDispatcher(
            IServiceProvider serviceProvider,
            ILogger<BufferingTradingMessageDispatcher> log,
            TradingMessageQueue queue)
        {
            _serviceProvider = serviceProvider;
            _log = log;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bufferTask = Task.Run(() => BufferMessageLoop(stoppingToken), stoppingToken);
            var sendTask = Task.Run(() => SendBatchMessageLoop(stoppingToken), stoppingToken);

            await Task.WhenAll(bufferTask, sendTask);
        }

        private async Task BufferMessageLoop(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    var message = await _queue.Dequeue(stoppingToken);
                    using (_bufferLocker.Lock(stoppingToken))
                    {
                        _buffer.Add(message);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(
                        ex,
                        $"{nameof(BufferingTradingMessageDispatcher)}.{nameof(BufferMessageLoop)} " +
                        "encountered an exception.");
                }
        }

        private async Task SendBatchMessageLoop(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    var bufferAge = _buffer.AgeAt(DateTime.UtcNow);
                    if (bufferAge < BufferingPeriod)
                    {
                        await Task.Delay(BufferingPeriod - bufferAge, stoppingToken);
                        continue;
                    }

                    Buffer buffer;
                    using (_bufferLocker.Lock(stoppingToken))
                    {
                        buffer = _buffer;
                        _buffer = new Buffer();
                    }

                    var addressedMessages = buffer.ToBatchMessages();

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var hubContext = scope.ServiceProvider.GetRequiredService<TradingHubContext>();
                        foreach (var addressedMessage in addressedMessages)
                        {
                            var connectionIds = addressedMessage.connectionId != null
                                ? new[] { addressedMessage.connectionId }
                                : Array.Empty<string>();

                            await hubContext.Send(addressedMessage.message, connectionIds);
                        }
                    }

                    await Task.Delay(BufferingPeriod, stoppingToken);
                }
                catch (Exception ex)
                {
                    _log.LogError(
                        ex,
                        $"{nameof(BufferingTradingMessageDispatcher)}.{nameof(SendBatchMessageLoop)} " +
                        "encountered an exception.");
                }
        }

        private sealed class Buffer
        {
            private readonly Dictionary<string, List<ITradingMessage>> _addressedMessages = new();
            private readonly List<ITradingMessage> _broadcastMessages = new();

            private DateTime? _startedAt;

            public TimeSpan AgeAt(DateTime utcDateTime)
            {
                var bufferAge = _startedAt != null
                    ? utcDateTime - _startedAt.Value
                    : TimeSpan.Zero;
                return bufferAge;
            }

            public void Add(BufferedMessage message)
            {
                if (_startedAt == null)
                    _startedAt = DateTime.UtcNow;

                if (message.IsBroadcast)
                {
                    _broadcastMessages.Add(message.Message);
                    foreach (var batch in _addressedMessages.Values)
                        batch.Add(message.Message);

                    return;
                }

                foreach (var connectionId in message.ConnectionIds)
                    _addressedMessages
                        .GetOrCreate(connectionId, _ => new List<ITradingMessage>())
                        .Add(message.Message);
            }

            public IReadOnlyList<(BatchMessage message, string? connectionId)> ToBatchMessages()
            {
                var batchMessages = new List<(BatchMessage message, string? connectionId)>();

                if (_broadcastMessages.Any())
                    batchMessages.Add((message: new BatchMessage(_broadcastMessages), connectionId: null));

                foreach (var batch in _addressedMessages)
                {
                    var messages = batch.Value;
                    if (!messages.Any())
                        continue;

                    batchMessages.Add((message: new BatchMessage(messages), connectionId: batch.Key));
                }

                return batchMessages;
            }
        }
    }
}
