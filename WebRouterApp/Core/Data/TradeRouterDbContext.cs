using Microsoft.EntityFrameworkCore;
using WebRouterApp.Features.Publishers.Application.Models;
using WebRouterApp.Features.Subscribers.Application.Models;
using WebRouterApp.Features.Users.Application.Models;

namespace WebRouterApp.Core.Data
{
    public class TradeRouterDbContext : DbContext
    {
        public TradeRouterDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<UserRecord> Users => Set<UserRecord>();
        public DbSet<PublisherRecord> Publishers => Set<PublisherRecord>();
        public DbSet<SubscriberRecord> Subscribers => Set<SubscriberRecord>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(TradeRouterDbContext).Assembly);
        }
    }
}
