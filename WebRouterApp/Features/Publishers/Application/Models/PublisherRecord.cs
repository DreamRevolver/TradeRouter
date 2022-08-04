using System;
using WebRouterApp.Core.Application.Models;

namespace WebRouterApp.Features.Publishers.Application.Models
{
    public class PublisherRecord : IEntity
    {
        public PublisherRecord(
            string apiKey,
            string apiSecret,
            string name,
            string description,
            bool tradeAllOrdersAsMarket)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            Name = name;
            Description = description;
            TradeAllOrdersAsMarket = tradeAllOrdersAsMarket;
        }

        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool TradeAllOrdersAsMarket { get; set; }

        public Guid Id { get; set; }
    }
}
