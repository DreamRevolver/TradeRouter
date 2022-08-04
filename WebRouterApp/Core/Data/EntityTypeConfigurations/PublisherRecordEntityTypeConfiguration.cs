using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebRouterApp.Features.Publishers.Application.Models;

namespace WebRouterApp.Core.Data.EntityTypeConfigurations
{
    internal class PublisherRecordEntityTypeConfiguration : IEntityTypeConfiguration<PublisherRecord>
    {
        public void Configure(EntityTypeBuilder<PublisherRecord> builder)
        {
            builder.ToTable("Publishers");

            builder.HasData(
                new PublisherRecord(
                    apiKey: "d270a941bc83bc337b475c755d9b99ce8374dd30f18d9c77d0fd758dff09d926",
                    apiSecret: "66ce28d2575e329f9fdfed24b9d2620b90278d5faecf8b01625601f4bd7cb884",
                    name: "7amuil",
                    description: "",
                    tradeAllOrdersAsMarket: false)
                {
                    Id = Guid.Parse("F9787C27-A4AE-488B-905B-2D58BB02C4AF")
                }
            );
        }
    }
}
