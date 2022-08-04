using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using WebRouterApp.Core.Data;
using WebRouterApp.Core.Infrastructure.ApiCallContextParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts;
using WebRouterApp.Features.Subscribers.Application.ErrorParts;
using WebRouterApp.Features.Subscribers.Application.Models;
using WebRouterApp.Features.Subscribers.Application.Services;
using WebRouterApp.Shared;
using WebRouterApp.Shared.Types;

namespace WebRouterApp.Features.Subscribers.Application.UseCases
{
    public sealed class UpdateSubscriberApiCommand : IApiRequest<Unit>
    {
        public Guid? SubscriberId { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public CoeffKinds? CoeffKind { get; set; }
        public double? Multiplier { get; set; }
        public TradeKinds? TradeKind { get; set; }
    }

    public sealed class UpdateSubscriberApiCommandValidator : AbstractValidator<UpdateSubscriberApiCommand>
    {
        public UpdateSubscriberApiCommandValidator()
        {
            RuleFor(it => it.SubscriberId).NotNull();
        }
    }

    public class UpdateSubscriberApiCommandHandler : IApiRequestHandler<UpdateSubscriberApiCommand, Unit>
    {
        private readonly IApiCallContextReader _apiCallContext;
        private readonly TradeRouterDbContext _dbContext;
        private readonly TraderLock _traderLock;

        public UpdateSubscriberApiCommandHandler(
            TradeRouterDbContext dbContext,
            IApiCallContextReader apiCallContext,
            TraderLock traderLock)
        {
            _dbContext = dbContext;
            _apiCallContext = apiCallContext;
            _traderLock = traderLock;
        }

        public async Task<Unit> Handle(UpdateSubscriberApiCommand command)
        {
            CodeAsserts.NotNull(command.SubscriberId);

            // We must lock the subscriber,
            // so it's not going to get started until we've finished updating it.
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
                                $"Cannot updated the subscriber '{subscriber.Record.Name}' while it is running."));
                }

                var record = _dbContext.Subscribers.Single(it => it.Id == command.SubscriberId);

                record.ApiKey = command.ApiKey ?? record.ApiKey;
                record.ApiSecret = command.ApiSecret ?? record.ApiSecret;
                record.Name = command.Name ?? record.Name;
                record.Description = command.Description ?? record.Description;
                record.CoeffKind = command.CoeffKind ?? record.CoeffKind;
                record.Multiplier = command.Multiplier ?? record.Multiplier;
                record.TradeKind = command.TradeKind ?? record.TradeKind;

                _dbContext.Update(record);
                await _dbContext.SaveChangesAsync();

                copySession?.Publisher.Update(record);
            }

            return Unit.Value;
        }
    }
}
