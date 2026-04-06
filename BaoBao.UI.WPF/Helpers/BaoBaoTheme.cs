using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BaoBao.UI.WPF.Helpers
{
    public static class BaoBaoTheme
    {
        private const string BundlePath = "Themes/BaoBao.xaml";
        private static readonly Uri BundleUri = new($"pack://application:,,,/BaoBao.UI.WPF;component/{BundlePath}", UriKind.Absolute);

        public static void UseDefaults()
        {
            UseDefaults(ThemeType.Light, "Default");
        }

        public static void UseDefaults(ThemeType theme)
        {
            UseDefaults(theme, "Default");
        }

        public static void UseDefaults(ThemeType theme, string paletteName)
        {
            EnsureResourcesLoaded();
            ThemeManager.SetTheme(theme);
            ThemeManager.SetPalette(paletteName);
        }

        public static void UseDefaults(ThemeType theme, BaoBaoPalette palette)
        {
            UseDefaults(theme, ThemeManager.PaletteToString(palette));
        }

        public static void UseDefaults(ThemeType theme, BaoBaoCustomPalette palette)
        {
            EnsureResourcesLoaded();
            ThemeManager.SetTheme(theme);
            ThemeManager.SetCustomPalette(palette);
        }

        public static void UseDefaults(ThemeType theme, Color primaryColor)
        {
            UseDefaults(theme, BaoBaoPaletteFactory.FromPrimaryColor(primaryColor));
        }

        public static void UseDefaults(ThemeType theme, string primaryHex, bool treatAsPrimaryColor)
        {
            if (!treatAsPrimaryColor)
            {
                UseDefaults(theme, primaryHex);
                return;
            }

            UseDefaults(theme, BaoBaoPaletteFactory.FromHex(primaryHex));
        }

        public static void EnsureResourcesLoaded()
        {
            if (Application.Current == null)
            {
                return;
            }

            var dictionaries = Application.Current.Resources.MergedDictionaries;
            if (dictionaries.Any(dict => UriEquals(dict.Source, BundleUri)))
            {
                return;
            }

            if (dictionaries.Any(ContainsBaoBaoThemeResources))
            {
                return;
            }

            dictionaries.Add(new ResourceDictionary { Source = BundleUri });
        }

        private static bool ContainsBaoBaoThemeResources(ResourceDictionary dictionary)
        {
            if (UriEquals(dictionary.Source, BundleUri))
            {
                return true;
            }

            if (dictionary.Source != null)
            {
                var source = dictionary.Source.ToString();
                if (source.IndexOf("/BaoBao.UI.WPF;component/Themes/", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    source.IndexOf("BaoBao.UI.WPF;component/Themes/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return dictionary.MergedDictionaries.Any(ContainsBaoBaoThemeResources);
        }

        private static bool UriEquals(Uri? left, Uri right)
        {
            return left != null && string.Equals(left.ToString(), right.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
