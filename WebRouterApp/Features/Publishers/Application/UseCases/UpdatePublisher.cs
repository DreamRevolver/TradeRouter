using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using WebRouterApp.Core.Data;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Subscribers.Application.Services;
using WebRouterApp.Shared;
using WebRouterApp.Shared.Types;

namespace WebRouterApp.Features.Publishers.Application.UseCases
{
    public sealed class UpdatePublisherApiCommand : IApiRequest<Unit>
    {
        public Guid? PublisherId { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? TradeAllOrdersAsMarket { get; set; }
    }

    public sealed class UpdatePublisherApiCommandValidator : AbstractValidator<UpdatePublisherApiCommand>
    {
        public UpdatePublisherApiCommandValidator()
        {
            RuleFor(it => it.PublisherId).NotNull();
        }
    }

    public class UpdatePublisherApiCommandHandler : IApiRequestHandler<UpdatePublisherApiCommand, Unit>
    {
        private readonly TradeRouterDbContext _dbContext;
        private readonly TraderLock _traderLock;

        public UpdatePublisherApiCommandHandler(
            TradeRouterDbContext dbContext,
            TraderLock traderLock)
        {
            _dbContext = dbContext;
            _traderLock = traderLock;
        }

        public async Task<Unit> Handle(UpdatePublisherApiCommand command)
        {
            CodeAsserts.NotNull(command.PublisherId);

            using (await _traderLock.For(command.PublisherId.Value).LockAsync())
            {
                // var copySession = _apiCallContext.Session;
                // if (copySession != null)
                // {
                //     var publisher = copySession.Publisher;
                //     if (publisher.IsRunning)
                //         throw new EndpointException(
                //             new PublisherRunningError(
                //                 publisher.Id,
                //                 $"Cannot updated the publisher '{publisher.Id}' while it is running."));
                // }

                var record = _dbContext.Publishers.Single(it => it.Id == command.PublisherId);

                record.ApiKey = command.ApiKey ?? record.ApiKey;
                record.ApiSecret = command.ApiSecret ?? record.ApiSecret;
                record.Name = command.Name ?? record.Name;
                record.Description = command.Description ?? record.Description;
                record.TradeAllOrdersAsMarket = command.TradeAllOrdersAsMarket ?? record.TradeAllOrdersAsMarket;

                _dbContext.Update(record);
                await _dbContext.SaveChangesAsync();

                // copySession?.Publisher.Update(record);
            }

            return Unit.Value;
        }
    }
}
