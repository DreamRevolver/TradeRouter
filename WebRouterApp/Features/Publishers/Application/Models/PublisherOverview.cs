using System;
using WebRouterApp.Core.Application.Models;

namespace WebRouterApp.Features.Publishers.Application.Models
{
    public class PublisherOverview
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = "";
        public DateTime? UtcStartedAt { get; init; }
        public int Ping { get; init; }
        public string FormattedBalance { get; init; } = "--";
        public string Description { get; init; } = "";
        public TraderStatus Status { get; init; }
    }
}
