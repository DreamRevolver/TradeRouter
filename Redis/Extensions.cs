using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Communication.Redis;

using static Communication.Redis.Constants;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

internal static class Extension
{

    internal static bool IsOkMessage(this ReadOnlyMemory<byte> message)
        => message.Length == OK_MESSAGE.Length && message.Span.SequenceEqual(OK_MESSAGE.Span);
    internal static bool IsOkMessage(this Memory<byte> message)
        => message.Length == OK_MESSAGE.Length && message.Span.SequenceEqual(OK_MESSAGE.Span);
    internal static bool IsOkMessage(this ReadOnlySpan<byte> message)
        => message.Length == OK_MESSAGE.Length && message.SequenceEqual(OK_MESSAGE.Span);
    internal static bool IsOkMessage(this Span<byte> message)
        => message.Length == OK_MESSAGE.Length && message.SequenceEqual(OK_MESSAGE.Span);
    internal static bool IsOkMessage(this byte[] message)
        => message.AsSpan().IsOkMessage();

    public static byte[] GetBytes(this string str)
        => Encoding.UTF8.GetBytes(str);
    public static string FromBytes(this byte[] bytes)
        => Encoding.UTF8.GetString(bytes);

    internal static string GetTopicsUnion(this string[] topics)
        => string.Join(".", topics);

    internal static byte[] GetSubscriptionPayload(this string[] topics)
        => $"PSUBSCRIBE {GetTopicsUnion(topics)}".GetBytes();
    internal static byte[] GetTopicsPublishPayload(this byte[] data, string[] topics)
        => $"PUBLISH {GetTopicsUnion(topics)} {Convert.ToBase64String(data)}".GetBytes();

    public static string GetLabelTopic<TEnum>(this TEnum label) where TEnum : Enum
        => label.GetChar().ToString();
    public static string GetLabelSubscriptionTopic<TEnum>(this TEnum label) where TEnum : Enum
        => $"[{string.Join("", label.AsEnumerable().Select(i => i.GetChar()))}]";


    public static IEnumerable<E> AsEnumerable<E>(this E label) where E : Enum
        => label.Equals(Label<E>.None) ? Enumerable.Empty<E>()
            : label.ToString().Split(',').Select(i => (E)Enum.Parse(typeof(E), i.Trim()));

    public static long AsLong<T>(this T label) where T : Enum
        => Convert.ToInt64(Enum.ToObject(typeof(T), label));
    public static T AsLabel<T>(this long number) where T : Enum
        => (T)Enum.ToObject(typeof(T), number);
    public static char GetChar<T>(this T label) where T : Enum
        => Label<T>.Base64CharMap[label];
    public static T GetLabel<T>(this int number) where T : Enum
        => (1L << number).AsLabel<T>();

}
