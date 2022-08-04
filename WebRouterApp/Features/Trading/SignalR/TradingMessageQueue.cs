using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using WebRouterApp.Features.Trading.Application.MessageParts;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Features.Trading.SignalR
{
    public class TradingMessageQueue
    {
        private readonly Channel<BufferedMessage> _channel;

        public TradingMessageQueue()
        {
            _channel = Channel.CreateUnbounded<BufferedMessage>();
        }

        public async ValueTask Enqueue(ITradingMessage message, params string[] connectionIds) =>
            await _channel.Writer.WriteAsync(new BufferedMessage(message, connectionIds));

        public async ValueTask<BufferedMessage> Dequeue(CancellationToken cancellationToken) => 
            await _channel.Reader.ReadAsync(cancellationToken);
    }
    

    public class BufferedMessage
    {
        public BufferedMessage(ITradingMessage message, IEnumerable<string> connectionIds)
        {
            Message = message;
            ConnectionIds = connectionIds.Distinct().ToHashSet();
        }

        public ITradingMessage Message { get; }
        public ISet<string> ConnectionIds { get; }
        public bool IsBroadcast => ConnectionIds.IsEmpty();
    }
}
