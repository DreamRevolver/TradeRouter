using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebRouterApp.Core.CopyEngineParts;
using WebRouterApp.Core.Infrastructure.ApiCallContextParts;
using WebRouterApp.Features.Trading.Application.MessageParts;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Features.Trading.SignalR
{
    public class TradingHub : Hub
    {
        private readonly IApiCallContextReader _apiCallContext;
        private readonly CopyEngine _copyEngine;

        public TradingHub(CopyEngine copyEngine, IApiCallContextReader apiCallContext)
        {
            _copyEngine = copyEngine;
            _apiCallContext = apiCallContext;
        }

        public override async Task OnConnectedAsync()
        {
            await _copyEngine.EnsureSessionCreated(_apiCallContext.CurrentUserId);
        }

        public string GetConnectionId() => Context.ConnectionId;
    }

    public class TradingHubContext
    {
        private readonly IHubContext<TradingHub> _hubContext;

        public TradingHubContext(IHubContext<TradingHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Send(
            BatchMessage message,
            params string[] connectionIds)
        {
            await ClientsFrom(connectionIds)
                .SendAsync(TradingHubClientMethods.BatchReceived, message);
        }

        private IClientProxy ClientsFrom(string[] connectionIds)
        {
            var clients = connectionIds.IsEmpty()
                ? _hubContext.Clients.All
                : _hubContext.Clients.Clients(connectionIds);

            return clients;
        }
    }

    public static class TradingHubClientMethods
    {
        public static readonly string BatchReceived = BatchMessage.ClientMethod;
    }
}
