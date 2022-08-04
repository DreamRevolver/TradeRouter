namespace SharedBinance.Models
{
	public readonly struct LeverageChange
	{
		public readonly string symbol;
		public readonly int value;
		public LeverageChange(string symbol, int value)
		{
			this.symbol = symbol;
			this.value = value;
		}
	}
}
