using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebRouterApp.Features.Subscribers.Application.Models;

namespace WebRouterApp.Core.Data.EntityTypeConfigurations
{
    internal class SubscriberRecordEntityTypeConfiguration : IEntityTypeConfiguration<SubscriberRecord>
    {
        public void Configure(EntityTypeBuilder<SubscriberRecord> builder)
        {
            builder.ToTable("Subscribers");
            
            builder.HasData(
                new SubscriberRecord(
                    apiKey: "3329ec7ae6bdffea7de1dd19a67a0b574f7a553c8e870bf9e062521e552b5615",
                    apiSecret: "eec08f26c98eb96268cc813afa159570c4441b490a3c72df85121b4baeab3a0b",
                    name: "Tom",
                    description: "Every Tom",
                    coeffKind: CoeffKinds.CoeffToSize,
                    multiplier: 1,
                    tradeKind: TradeKinds.TradeAsMarket)
                {
                    Id = Guid.Parse("3799DB3F-16F0-4A4F-8BD1-7D8E054F9F8B"),
                },
                new SubscriberRecord(
                    apiKey: "a552521f2823e6583418462ff8e9d094ac6dda958bea017863f88d82908ea691",
                    apiSecret: "6a6bd42fb8f3d956f6642962be712cc015d1736eebcb48cd2e974a7833a430af",
                    name: "Dick",
                    description: "Every Dick",
                    coeffKind: CoeffKinds.CoeffToSize,
                    multiplier: 1,
                    tradeKind: TradeKinds.TradeAsMarket)
                {
                    Id = Guid.Parse("E6BA2381-D4F3-4DAD-88A0-E470A0F60377"),
                },
                new SubscriberRecord(
                    apiKey: "9cbca5494d0e66652aa933dfd16585e46ebb488ee10ea528f3be7684739ad4df",
                    apiSecret: "50611c69847096dca50e1eaef774f884e593f30ccd82f4249fcc743e9b479a75",
                    name: "User1 (0.75)",
                    description: "user1@zgmd.onmicrosoft.com",
                    coeffKind: CoeffKinds.CoeffToSize,
                    multiplier: 1,
                    tradeKind: TradeKinds.TradeAsMarket)
                {
                    Id = Guid.Parse("2B389379-4AA3-481B-B54E-05156BA6BE8C"),
                },
                new SubscriberRecord(
                    apiKey: "186f0d31c9b33a1426a5b2c70d7703e87c3a3704f947f2c2815a9e9194601bfa",
                    apiSecret: "a8b90468eaf611228f2174a5eadbd1f1d0b9a8cb0f03e3be36ba425acc2978e2",
                    name: "User2 (0.5)",
                    description: "user2@zgmd.onmicrosoft.com",
                    coeffKind: CoeffKinds.CoeffToSize,
                    multiplier: 1,
                    tradeKind: TradeKinds.TradeAsMarket)
                {
                    Id = Guid.Parse("A2EF6E28-343D-4BE2-86C2-BC235693F512"),
                }
            );
        }
    }
}
