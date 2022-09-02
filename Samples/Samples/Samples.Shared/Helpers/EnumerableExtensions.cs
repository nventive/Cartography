using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samples.Helpers
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
    }
}