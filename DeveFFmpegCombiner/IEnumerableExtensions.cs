using System.Collections.Generic;
using System.Linq;

namespace DeveFFmpegCombiner
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> TakeNthItems<T>(this IEnumerable<T> ienumerable, int nth)
        {
            return ienumerable.Where((x, i) => i % nth == 0);
        }
    }
}
