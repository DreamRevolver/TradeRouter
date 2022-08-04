using System;
namespace SharedBinance.Extensions
{
	public static class CompareExtension
	{
		public static int NullableCompare<T>(this T first, T second) where T : IComparable<T>
			=> first is null || second is null ? first is null && second is null ? 0 :
				first is null ? -1 : 1 : first.CompareTo(second);
	}
}
