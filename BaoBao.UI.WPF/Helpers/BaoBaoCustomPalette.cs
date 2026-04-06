using System.Windows.Media;

namespace BaoBao.UI.WPF.Helpers
{
    public sealed class BaoBaoCustomPalette
    {
        public Color? Primary { get; set; }
        public Color? PrimaryHover { get; set; }
        public Color? PrimaryPressed { get; set; }

        public Color? Success { get; set; }
        public Color? Info { get; set; }
        public Color? Warning { get; set; }
        public Color? Error { get; set; }

        public Color? BackgroundLight { get; set; }
        public Color? BackgroundDark { get; set; }
        public Color? SurfaceLight { get; set; }
        public Color? SurfaceDark { get; set; }

        public Color? FillLight { get; set; }
        public Color? FillDark { get; set; }
        public Color? HeaderLight { get; set; }
        public Color? HeaderDark { get; set; }

        public Color? TextPrimaryLight { get; set; }
        public Color? TextPrimaryDark { get; set; }
        public Color? TextSecondaryLight { get; set; }
        public Color? TextSecondaryDark { get; set; }
        public Color? TextPlaceHolderLight { get; set; }
        public Color? TextPlaceHolderDark { get; set; }

        public Color? BorderLight { get; set; }
        public Color? BorderDark { get; set; }
    }
}
