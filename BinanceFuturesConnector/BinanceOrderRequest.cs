using System.Runtime.Serialization;

namespace BinanceFuturesConnector
{
    public sealed class BinanceOrderRequest
    {
        [DataMember(Name = "symbol")]
		public string symbol  { get; set; }
        [DataMember(Name = "side")]
		public string side  { get; set; }
        [DataMember(Name = "type")]
		public string type  { get; set; }
        [DataMember(Name = "quantity")]
		public double? quantity  { get; set; }
        [DataMember(Name = "price")]
		public double? price  { get; set; }
        [DataMember(Name = "stopPrice")]
		public double? stopPrice  { get; set; }
        [DataMember(Name = "timeInForce")]
		public string timeInForce  { get; set; }
        [DataMember(Name = "closePosition")]
		public bool? closePosition  { get; set; }
        [DataMember(Name = "callbackRate")]
		public double? callbackRate  { get; set; }
        [DataMember(Name = "newClientOrderId")]
		public string newClientOrderId  { get; set; }

    }
}
