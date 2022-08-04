using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using WebRouterApp.Core.CopyEngineParts.TraderParts;
using WebRouterApp.Core.Infrastructure.ApiCallContextParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Trading.Application.MessageParts;
using WebRouterApp.Features.Trading.SignalR;
using WebRouterApp.Shared;
using WebRouterApp.Shared.Types;

namespace WebRouterApp.Features.Trading.Application.UseCases
{
    public sealed class  RequestTradingSnapshotApiCommand : IApiRequest<Unit>
    {
        // The SignalR connection to send orders and positions to. 
        public string? ConnectionId { get; set; }
    }

    public sealed class RequestOrdersPositionsApiCommandValidator : AbstractValidator<RequestTradingSnapshotApiCommand>
    {
        public RequestOrdersPositionsApiCommandValidator()
        {
            RuleFor(it => it.ConnectionId).Must(value => !string.IsNullOrWhiteSpace(value));
        }
    }
    
    public class RequestTradingSnapshotApiCommandHandler : IApiRequestHandler<RequestTradingSnapshotApiCommand, Unit>
    {
        private readonly ILogger<RequestTradingSnapshotApiCommandHandler> _logger;
        private readonly TradingMessageQueue _tradingMessageQueue;
        private readonly IApiCallContextReader _apiCallContext;

        public RequestTradingSnapshotApiCommandHandler(
            ILogger<RequestTradingSnapshotApiCommandHandler> logger,
            TradingMessageQueue tradingMessageQueue, 
            IApiCallContextReader apiCallContext)
        {
            _logger = logger;
            _tradingMessageQueue = tradingMessageQueue;
            _apiCallContext = apiCallContext;
        }

        public Task<Unit> Handle(RequestTradingSnapshotApiCommand command)
        {
            if (_apiCallContext.Session == null)
                return Task.FromResult(Unit.Value);
            
            CodeAsserts.NotNull(command.ConnectionId);
            
            var publisher = _apiCallContext.Session.Publisher;
            
            PostTradingSnapshot(command.ConnectionId, publisher);
            foreach (var subscriber in publisher.RunningSubscribers)
                PostTradingSnapshot(command.ConnectionId, subscriber);

            return Task.FromResult(Unit.Value);
        }

        private void PostTradingSnapshot(string connectionId, Trader trader)
        {
            _ = _tradingMessageQueue.Enqueue(
                Messages.OrdersSnapshot(traderId: trader.Id, trader.Orders), 
                connectionId);

            _ = _tradingMessageQueue.Enqueue(
                Messages.PositionsSnapshot(traderId: trader.Id, trader.Positions), 
                connectionId);
        }
    }
}
