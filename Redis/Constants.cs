using System;
using System.Collections.Immutable;
using System.Linq;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Communication.Redis
{
	public static class Constants
	{

		internal const int DEFAULT_BUFFER_SIZE = 2048;
		public const int DEFAULT_PORT = 6379;
		internal static readonly byte[] _space = " ".GetBytes();
		internal static ReadOnlyMemory<byte> SPACE = _space.AsMemory();
		internal static readonly byte[] _separator = "\r\n".GetBytes();
		internal static ReadOnlyMemory<byte> SEPARATOR => _separator.AsMemory();
		private static readonly byte[] _okMessage = "+OK\r\n".GetBytes();
		internal static ReadOnlyMemory<byte> OK_MESSAGE => _okMessage.AsMemory();
		private static readonly byte[] _publishPrefix = "PUBLISH".GetBytes();
		internal static readonly ReadOnlyMemory<byte> PUBLISH_PREFIX = _publishPrefix.AsMemory();
		private static readonly byte[] _psubscribePrefix = "PSUBSCRIBE".GetBytes();
		internal static readonly ReadOnlyMemory<byte> PSUBSCRIBE_PREFIX = _psubscribePrefix.AsMemory();

	}

	internal static class Label<TEnum> where TEnum : Enum
	{

		private const string BASE64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

		static Label()
		{
			Any = (TEnum) Enum.ToObject(
				typeof(TEnum), Enum.GetValues(typeof(TEnum))
					.Cast<TEnum>()
					.Select(i => Convert.ToInt64(i))
					.Aggregate((a, b) => a | b)
			);
			None = (TEnum) Enum.ToObject(typeof(TEnum), 0);
			Size = Enum.GetValues(typeof(TEnum)).Length;
			Base64CharMap = Enum.GetValues(typeof(TEnum))
				.Cast<TEnum>()
				.OrderBy(i => i.AsLong())
				.Select((i, n) => (label: i, number: n))
				.ToImmutableDictionary(i => i.label, i => BASE64[i.number]);
		}

		internal static ImmutableDictionary<TEnum, char> Base64CharMap { get; }
		public static TEnum Any { get; }
		public static TEnum None { get; }

		// ReSharper disable once StaticMemberInGenericType
		public static int Size { get; }

	}

}
