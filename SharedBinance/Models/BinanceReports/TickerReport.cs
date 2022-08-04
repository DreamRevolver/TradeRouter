using Newtonsoft.Json;

namespace SharedBinance.Models.BinanceReports
{

	public sealed class TickerReport
	{

		[JsonProperty("u")]
		public long UpdateId;

		[JsonProperty("s")]
		public string Symbol;

		[JsonProperty("b")]
		public double BidPx;

		[JsonProperty("B")]
		public double BidQty;

		[JsonProperty("a")]
		public double AskPx;

		[JsonProperty("A")]
		public double AskQty;

		[JsonProperty("T")]
		public long TransactionTime;

		[JsonProperty("E")]
		public long EventTime;

		[JsonProperty("e")]
		public string EventType;

	}

}
