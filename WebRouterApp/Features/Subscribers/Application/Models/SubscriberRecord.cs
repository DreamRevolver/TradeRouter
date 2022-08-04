using System;
using WebRouterApp.Core.Application.Models;

namespace WebRouterApp.Features.Subscribers.Application.Models
{
    public class SubscriberRecord : IEntity
    {
        public SubscriberRecord(
            string apiKey, 
            string apiSecret, 
            string name,
            string description,
            CoeffKinds coeffKind, 
            double multiplier, 
            TradeKinds tradeKind)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            Name = name;
            Description = description;
            CoeffKind = coeffKind;
            Multiplier = multiplier;
            TradeKind = tradeKind;
        }

        public Guid Id { get; set; }
        
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CoeffKinds CoeffKind { get; set; }
        public double Multiplier { get; set; }
        public TradeKinds TradeKind { get; set; }
    }

    public enum CoeffKinds
    {
        CoeffToSize = 0,
    }

    public enum TradeKinds
    {
        TradeAsMarket = 0,
    }
}
