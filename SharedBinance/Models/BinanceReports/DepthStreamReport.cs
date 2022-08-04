using Newtonsoft.Json;
namespace SharedBinance.Models.BinanceReports
{
    public sealed class DepthStreamReport
    {

        [JsonProperty("e")]
        public string EventType;

        [JsonProperty("E")]
        public long EventTime;

        [JsonProperty("T")]
        public long TransactionTime;

        [JsonProperty("s")]
        public string Symbol;

        [JsonProperty("U")]
        public long FirstUpdateId;

        [JsonProperty("u")]
        public long FinalUpdateId;

        [JsonProperty("pu")]
        public long FinalUpdateStreamId;

        [JsonProperty("b")]
        public double[][] bidUpdates;

        [JsonProperty("a")]
        public double[][] askUpdates;
    }
}
