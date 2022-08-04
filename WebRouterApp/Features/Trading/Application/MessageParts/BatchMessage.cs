using System.Collections.Generic;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Features.Trading.Application.MessageParts
{
    public class BatchMessage : ITradingMessage
    {
        public static readonly string ClientMethod = "BatchReceived";

        public BatchMessage(IEnumerable<ITradingMessage> messages)
        {
            Messages = messages.ToReadOnlyList();
        }

        public IReadOnlyList<object> Messages { get; }

        public string Tag => "Trading.Batch";
    }
}
