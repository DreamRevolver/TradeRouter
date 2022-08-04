using Newtonsoft.Json;
namespace SharedBinance.Models.BinanceReports
{
    public sealed class SnapshotReport
    {

        [JsonProperty("lastUpdateId")]
        public long lastUpdateId;

        [JsonProperty("E")]
        public long MessageTimestamp;

        [JsonProperty("T")]
        public long TransactionTimestamp;

        [JsonProperty("bids")]
        public double[][] bids;

        [JsonProperty("asks")]
        public double[][] asks;

    }

}
