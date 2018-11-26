using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeveFFmpegCombiner
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> TakeNthItems<T>(this IEnumerable<T> ienumerable, int nth)
        {
            return ienumerable.Where((x, i) => i % nth == 0);
        }

        public static IEnumerable<string> CustomSort(this IEnumerable<string> list)
        {
            if (!list.Any())
            {
                return list;
            }

            int maxLen = list.Select(s => s.Length).Max();

            return list.Select(s => new
            {
                OrgStr = s,
                SortStr = Regex.Replace(s, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
            })
            .OrderBy(x => x.SortStr)
            .Select(x => x.OrgStr);
        }
    }
}
