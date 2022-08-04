namespace SharedBinance.Models
{
    public sealed class SlaveIdentity
    {
		public string ApiKey { get; set; }
		public string ApiSecret { get; set; }
		public string Name { get; set; }
		public string Mode { get; set; }
		public string ModeValue { get; set; }
		public string CopyMasterOrders { get; set; }
		public string Url { get; set; }
		public string Wss { get; set; }
	}
}
