using System;
using System.Windows.Media;

namespace BaoBao.UI.WPF.Helpers
{
    public static class BaoBaoPaletteFactory
    {
        public static BaoBaoCustomPalette FromPrimaryColor(Color primary)
        {
            var hover = Blend(primary, Colors.White, 0.18);
            var pressed = Blend(primary, Colors.Black, 0.18);
            var backgroundLight = Blend(primary, Colors.White, 0.94);
            var backgroundDark = Blend(primary, Colors.Black, 0.82);
            var surfaceLight = Blend(primary, Colors.White, 0.975);
            var surfaceDark = Blend(primary, Colors.Black, 0.75);
            var fillLight = Blend(primary, Colors.White, 0.96);
            var fillDark = Blend(primary, Colors.Black, 0.85);
            var headerLight = Blend(primary, Colors.White, 0.9);
            var headerDark = Blend(primary, Colors.Black, 0.67);

            return new BaoBaoCustomPalette
            {
                Primary = primary,
                PrimaryHover = hover,
                PrimaryPressed = pressed,
                Success = primary,
                Info = primary,
                Warning = Color.FromRgb(240, 160, 32),
                Error = Color.FromRgb(208, 48, 80),
                BackgroundLight = backgroundLight,
                BackgroundDark = backgroundDark,
                SurfaceLight = surfaceLight,
                SurfaceDark = surfaceDark,
                FillLight = fillLight,
                FillDark = fillDark,
                HeaderLight = headerLight,
                HeaderDark = headerDark,
                TextPrimaryLight = Blend(primary, Colors.Black, 0.72),
                TextPrimaryDark = Blend(primary, Colors.White, 0.9),
                TextSecondaryLight = Blend(primary, Colors.Gray, 0.45),
                TextSecondaryDark = Blend(primary, Colors.White, 0.55),
                TextPlaceHolderLight = Blend(primary, Colors.LightGray, 0.55),
                TextPlaceHolderDark = Blend(primary, Colors.Gray, 0.5),
                BorderLight = Blend(primary, Colors.White, 0.78),
                BorderDark = Blend(primary, Colors.Gray, 0.42)
            };
        }

        public static BaoBaoCustomPalette FromHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                throw new ArgumentException("Hex color cannot be null or empty.", nameof(hex));
            }

            var color = (Color)ColorConverter.ConvertFromString(hex);
            return FromPrimaryColor(color);
        }

        private static Color Blend(Color source, Color target, double ratio)
        {
            ratio = Math.Max(0, Math.Min(1, ratio));

            byte a = BlendChannel(source.A, target.A, ratio);
            byte r = BlendChannel(source.R, target.R, ratio);
            byte g = BlendChannel(source.G, target.G, ratio);
            byte b = BlendChannel(source.B, target.B, ratio);

            return Color.FromArgb(a, r, g, b);
        }

        private static byte BlendChannel(byte from, byte to, double ratio)
        {
            return (byte)Math.Round(from + ((to - from) * ratio));
        }
    }
}
