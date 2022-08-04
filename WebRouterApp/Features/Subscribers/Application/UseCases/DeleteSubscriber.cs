using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using WebRouterApp.Core.Data;
using WebRouterApp.Core.Infrastructure.ApiCallContextParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts;
using WebRouterApp.Features.Subscribers.Application.ErrorParts;
using WebRouterApp.Features.Subscribers.Application.Services;
using WebRouterApp.Shared;
using WebRouterApp.Shared.Types;

namespace WebRouterApp.Features.Subscribers.Application.UseCases
{
    public sealed class DeleteSubscriberApiCommand : IApiRequest<Unit>
    {
        public Guid? SubscriberId { get; set; }
    }

    public sealed class DeleteSubscriberApiCommandValidator : AbstractValidator<DeleteSubscriberApiCommand>
    {
        public DeleteSubscriberApiCommandValidator()
        {
            RuleFor(it => it.SubscriberId).NotNull();
        }
    }

    public class DeleteSubscriberApiCommandHandler : IApiRequestHandler<DeleteSubscriberApiCommand, Unit>
    {
        private readonly IApiCallContextReader _apiCallContext;
        private readonly TradeRouterDbContext _dbContext;
        private readonly TraderLock _traderLock;

        public DeleteSubscriberApiCommandHandler(TradeRouterDbContext dbContext,
            TraderLock traderLock,
            IApiCallContextReader apiCallContext)
        {
            _dbContext = dbContext;
            _traderLock = traderLock;
            _apiCallContext = apiCallContext;
        }

        public async Task<Unit> Handle(DeleteSubscriberApiCommand command)
        {
            CodeAsserts.NotNull(command.SubscriberId);

            // We must lock the subscriber,
            // so it's not going to get started while we're deleting it.
            using (await _traderLock.For(command.SubscriberId.Value).LockAsync())
            {
                var copySession = _apiCallContext.Session;
                if (copySession != null)
                {
                    var subscriber =
                        copySession.Publisher.Subscribers.FirstOrDefault(it => it.Id == command.SubscriberId);
                    
                    if (subscriber == null)
                        throw new EndpointException(
                            new NotFoundError(
                                command.SubscriberId.Value,
                                $"The subscriber '{command.SubscriberId}' was not found."));

                    if (subscriber.IsRunning)
                        throw new EndpointException(
                            new SubscriberRunningError(
                                subscriber.Id,
                                $"Cannot delete the subscriber '{subscriber.Record.Name}' while it is running."));
                
                    copySession.Publisher.Remove(subscriber.Id);
                }

                var record = _dbContext.Subscribers.FirstOrDefault(it => it.Id == command.SubscriberId);
                if (record == null)
                    return Unit.Value;

                _dbContext.Remove(record);
                await _dbContext.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
