using Newtonsoft.Json;
namespace SharedBinance.Models.BinanceReports
{
    public sealed class BalanceReport
    {

        [JsonProperty("a")]
        public string Asset;

        [JsonProperty("wb")]
        public double WalletBalance;

        [JsonProperty("cw")]
        public double CrossWalletBalance;

    }

}
