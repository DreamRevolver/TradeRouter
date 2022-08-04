using System;
using System.Threading.Tasks;
using FluentValidation;
using WebRouterApp.Core.Data;
using WebRouterApp.Core.Infrastructure.ApiCallContextParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Subscribers.Application.Models;
using WebRouterApp.Shared;

namespace WebRouterApp.Features.Subscribers.Application.UseCases
{
    public sealed class CreateSubscriberApiCommand : IApiRequest<CreateSubscriberApiResponse>
    {
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public CoeffKinds? CoeffKind { get; set; }
        public double? Multiplier { get; set; }
        public TradeKinds? TradeKind { get; set; }
    }

    public sealed record CreateSubscriberApiResponse(Guid Id);

    public sealed class CreateSubscriberApiCommandValidator : AbstractValidator<CreateSubscriberApiCommand>
    {
        public CreateSubscriberApiCommandValidator()
        {
            // We'll generate a Guid for Subscriber.Id ourselves.

            RuleFor(it => it.ApiKey).Must(value => !string.IsNullOrWhiteSpace(value));
            RuleFor(it => it.ApiSecret).Must(value => !string.IsNullOrWhiteSpace(value));
            RuleFor(it => it.Name).Must(value => !string.IsNullOrWhiteSpace(value));
            RuleFor(it => it.Description).Must(value => !string.IsNullOrWhiteSpace(value));
            RuleFor(it => it.CoeffKind).NotNull();
            RuleFor(it => it.Multiplier).NotNull();
            RuleFor(it => it.TradeKind).NotNull();
        }
    }

    public class CreateSubscriberApiCommandHandler
        : IApiRequestHandler<CreateSubscriberApiCommand, CreateSubscriberApiResponse>
    {
        private readonly IApiCallContextReader _apiCallContext;
        private readonly TradeRouterDbContext _dbContext;

        public CreateSubscriberApiCommandHandler(TradeRouterDbContext dbContext, IApiCallContextReader apiCallContext)
        {
            _dbContext = dbContext;
            _apiCallContext = apiCallContext;
        }

        public Task<CreateSubscriberApiResponse> Handle(CreateSubscriberApiCommand command)
        {
            CodeAsserts.NotNull(command.ApiKey);
            CodeAsserts.NotNull(command.ApiSecret);
            CodeAsserts.NotNull(command.Name);
            CodeAsserts.NotNull(command.Description);
            CodeAsserts.NotNull(command.CoeffKind);
            CodeAsserts.NotNull(command.Multiplier);
            CodeAsserts.NotNull(command.TradeKind);

            var record = new SubscriberRecord(
                command.ApiKey,
                command.ApiSecret,
                command.Name,
                command.Description,
                command.CoeffKind.Value,
                command.Multiplier.Value,
                command.TradeKind.Value);

            _dbContext.Add(record);
            _dbContext.SaveChanges();

            _apiCallContext.Session?.Publisher.Add(record);

            return Task.FromResult(new CreateSubscriberApiResponse(record.Id));
        }
    }
}
