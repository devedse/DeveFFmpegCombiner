using System.Drawing;

namespace DeveFFmpegCombiner
{
    public static class RectangleExtensions
    {
        public static string ToFFmpegStr(this Rectangle rect)
        {
            return $"{rect.Width}:{rect.Height}:{rect.X}:{rect.Y}";
        }
    }
}
