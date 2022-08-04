using System;
using Shared.Models;
namespace SharedBinance.Models
{
    public class Instrument
    {
        public string Exchange { get; set; }
        public string Symbol { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public string MarginAsset { get; set; }
        public long? PricePrecision { get; set; }
        public long? QuantityPrecision { get; set; }
        public long? BaseAssetPrecision { get; set; }
        public long? QuotePrecision { get; set; }
        public ContractType? ContractType { get; set; }
        public DateTime? DeliveryDate { get; set; }


        public string Id()
            => Id(Exchange, null, null, Symbol);

        public static string Id(string exchange, string first, string second, string Symbol, DateTime? ExpirationTime = null)
        {
            if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(second))
            {
                return $"{exchange}-{Symbol}";
            }

            if (ExpirationTime == null)
            {
                return $"{exchange}-{first}-{second}";
            }
            return $"{exchange}-{first}-{second}-{ExpirationTimeToString((DateTime)ExpirationTime)}";
        }

        public static string ExpirationTimeToString(DateTime time)
            => $"{time.ToString("yyMMdd")}";

    }
}
