using System;
using System.Windows.Media;

namespace Transport.Results.Extensions
{
    public static class ColorExtensions
    {
        public static String ToHtml(this Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}