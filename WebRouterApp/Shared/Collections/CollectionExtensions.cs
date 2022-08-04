using System;
using System.Collections.Generic;
using System.Linq;

namespace WebRouterApp.Shared.Collections
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> createValue)
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = createValue(key);
                dictionary.Add(key, value);
            }

            return value;
        }
    }

    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            foreach (var it in range)
                collection.Add(it);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            foreach (var it in range)
                collection.Remove(it);
        }
    }

    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
            => !enumerable.Any();

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> enumerable)
            => enumerable.ToList();

        public static IReadOnlyList<T> AsReadOnlyList<T>(this List<T> list)
            => list;
    }

    public static class EmptyReadOnlyList<T>
    {
        public static readonly IReadOnlyList<T> Instance = new List<T>();
    }
}
