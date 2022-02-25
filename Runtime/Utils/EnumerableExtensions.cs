using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) { return source == null || !source.Any(); }
    }
}
