using System;

namespace SharedBinance.Models
{

	public struct MarketBook
	{

		public DateTime Time { get; set; }
		public double Bid { get; set; }
		public double Ask { get; set; }
		public double Trade { get; set; }

		public readonly double Last
			=> Trade != 0 ? Trade : (Bid + Ask) / 2;

	}

}
