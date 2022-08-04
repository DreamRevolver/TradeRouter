using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SharedBinance.Models
{

	[DebuggerDisplay(nameof(Balance) + ": Currency={Currency} Value={Value}")]
	public sealed class Balance
	{

		public static IEqualityComparer<Balance> CompareSymbol { get; } = new BalanceSymbolComparer();

		[DataMember(Name = "Value")]
		public double Value { set; get; }

		[DataMember(Name = "Currency")]
		public string Currency { set; get; }

		[DataMember(Name = "Type")]
		public string Type { set; get; }

		[DataMember(Name = "BalanceChange")]
		public string BalanceChange { set; get; }

		[DataMember(Name = "lastUpdate")]
		public long lastUpdate { set; get; }

		private class BalanceSymbolComparer : IEqualityComparer<Balance>
		{

			public bool Equals(Balance x, Balance y) => x.Currency.Equals(y.Currency);
			public int GetHashCode(Balance obj) => obj.GetHashCode();

		}

	}

}
