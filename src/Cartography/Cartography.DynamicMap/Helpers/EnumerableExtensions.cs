using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cartography.DynamicMap.Helpers
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Safe<T>(this IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }

        public static bool SafeEquals<T>(this T obj, T other)
           where T : class
        {
            if (obj == null)
            {
                return other == null;
            }
            else
            {
                return obj.Equals(other);
            }
        }

        public static bool SafeSequenceEqual<T>(this List<T> source, List<T> other) =>
            (source ?? new List<T>()).SequenceEqual(other ?? new List<T>());

        public static bool SafeSequenceEqual<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> other) =>
            (source ?? new Dictionary<TKey, TValue>()).SequenceEqual(other ?? new Dictionary<TKey, TValue>());
    }
}