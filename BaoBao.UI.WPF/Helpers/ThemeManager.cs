using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace BaoBao.UI.WPF.Helpers
{
    public enum ThemeType
    {
        Light,
        Dark
    }

    public static class ThemeManager
    {
        private const string CustomDictionaryTagKey = "__BaoBaoThemeTag";
        public static ThemeType CurrentTheme { get; private set; } = ThemeType.Light;
        public static string CurrentPalette { get; private set; } = "Default";
        public static BaoBaoPaletteMode CurrentPaletteMode { get; private set; } = BaoBaoPaletteMode.BuiltIn;
        public static BaoBaoPalette? CurrentBuiltInPalette { get; private set; } = BaoBaoPalette.Default;
        public static BaoBaoCustomPalette? CurrentCustomPalette { get; private set; }

        public static void SetTheme(ThemeType theme)
        {
            BaoBaoTheme.EnsureResourcesLoaded();

            string themeFile = theme == ThemeType.Light ? "Themes/Theme.Light.xaml" : "Themes/Theme.Dark.xaml";

            UpdateDictionary("Theme", themeFile);
            CurrentTheme = theme;
            ReapplyCurrentPalette();
        }

        public static void SetPalette(string paletteName)
        {
            BaoBaoTheme.EnsureResourcesLoaded();

            string paletteFile = string.IsNullOrEmpty(paletteName) || paletteName.Equals("Default", StringComparison.OrdinalIgnoreCase)
                ? "Themes/Colors.xaml"
                : $"Themes/Palettes/{paletteName}.xaml";

            UpdateDictionary("Palette", paletteFile);
            CurrentPalette = paletteName;
            CurrentPaletteMode = BaoBaoPaletteMode.BuiltIn;
            CurrentBuiltInPalette = StringToPalette(paletteName);
            CurrentCustomPalette = null;
        }

        public static void SetPalette(BaoBaoPalette palette)
        {
            SetPalette(PaletteToString(palette));
        }

        public static void SetCustomPalette(BaoBaoCustomPalette palette)
        {
            BaoBaoTheme.EnsureResourcesLoaded();

            UpdateDictionary("Palette", CreateCustomPaletteDictionary(palette));
            CurrentPalette = "Custom";
            CurrentPaletteMode = BaoBaoPaletteMode.Custom;
            CurrentBuiltInPalette = null;
            CurrentCustomPalette = ClonePalette(palette);
        }

        public static void SetPaletteFromPrimaryColor(Color primary)
        {
            SetCustomPalette(BaoBaoPaletteFactory.FromPrimaryColor(primary));
        }

        public static void SetPaletteFromHex(string hex)
        {
            SetCustomPalette(BaoBaoPaletteFactory.FromHex(hex));
        }

        private static void UpdateDictionary(string tag, string filePath)
        {
            if (Application.Current == null)
            {
                return;
            }

            var uri = new Uri($"pack://application:,,,/BaoBao.UI.WPF;component/{filePath}");
            var dictionaries = Application.Current.Resources.MergedDictionaries;

            if (TryFindDictionaryOwner(dictionaries, tag, out var ownerCollection, out var index))
            {
                ownerCollection[index] = new ResourceDictionary { Source = uri };
                return;
            }

            if (tag == "Palette")
            {
                dictionaries.Insert(0, new ResourceDictionary { Source = uri });
            }
            else
            {
                dictionaries.Add(new ResourceDictionary { Source = uri });
            }
        }

        private static void UpdateDictionary(string tag, ResourceDictionary dictionary)
        {
            if (Application.Current == null)
            {
                return;
            }

            var dictionaries = Application.Current.Resources.MergedDictionaries;
            dictionary[CustomDictionaryTagKey] = tag;

            if (TryFindDictionaryOwner(dictionaries, tag, out var ownerCollection, out var index))
            {
                ownerCollection[index] = dictionary;
                return;
            }

            if (tag == "Palette")
            {
                dictionaries.Insert(0, dictionary);
            }
            else
            {
                dictionaries.Add(dictionary);
            }
        }

        private static bool TryFindDictionaryOwner(
            ICollection<ResourceDictionary> dictionaries,
            string tag,
            out IList<ResourceDictionary> ownerCollection,
            out int index)
        {
            if (dictionaries is IList<ResourceDictionary> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var dict = list[i];
                    if (IsTargetDictionary(dict, tag))
                    {
                        ownerCollection = list;
                        index = i;
                        return true;
                    }

                    if (TryFindDictionaryOwner(dict.MergedDictionaries, tag, out ownerCollection, out index))
                    {
                        return true;
                    }
                }
            }

            ownerCollection = null!;
            index = -1;
            return false;
        }

        private static bool IsTargetDictionary(ResourceDictionary dictionary, string tag)
        {
            if (dictionary.Contains(CustomDictionaryTagKey) &&
                string.Equals(dictionary[CustomDictionaryTagKey] as string, tag, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (dictionary.Source == null)
            {
                return false;
            }

            var source = dictionary.Source.ToString();
            if (tag == "Theme")
            {
                return source.EndsWith("Themes/Theme.Light.xaml", StringComparison.OrdinalIgnoreCase) ||
                       source.EndsWith("Themes/Theme.Dark.xaml", StringComparison.OrdinalIgnoreCase);
            }

            if (tag == "Palette")
            {
                return source.EndsWith("Themes/Colors.xaml", StringComparison.OrdinalIgnoreCase) ||
                       source.IndexOf("Themes/Palettes/", StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return false;
        }

        private static ResourceDictionary CreateCustomPaletteDictionary(BaoBaoCustomPalette palette)
        {
            var dictionary = new ResourceDictionary();
            dictionary.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/BaoBao.UI.WPF;component/Themes/Colors.xaml", UriKind.Absolute)
            });

            SetColor(dictionary, "BaoBao_Color_Primary", palette.Primary);
            SetColor(dictionary, "BaoBao_Color_Primary_Hover", palette.PrimaryHover);
            SetColor(dictionary, "BaoBao_Color_Primary_Pressed", palette.PrimaryPressed);

            SetColor(dictionary, "BaoBao_Color_Success", palette.Success);
            SetColor(dictionary, "BaoBao_Color_Info", palette.Info);
            SetColor(dictionary, "BaoBao_Color_Warning", palette.Warning);
            SetColor(dictionary, "BaoBao_Color_Error", palette.Error);

            SetColor(dictionary, "BaoBao_Color_Background_Light", palette.BackgroundLight);
            SetColor(dictionary, "BaoBao_Color_Background_Dark", palette.BackgroundDark);
            SetColor(dictionary, "BaoBao_Color_Surface_Light", palette.SurfaceLight);
            SetColor(dictionary, "BaoBao_Color_Surface_Dark", palette.SurfaceDark);

            SetColor(dictionary, "BaoBao_Color_Fill_Light", palette.FillLight);
            SetColor(dictionary, "BaoBao_Color_Fill_Dark", palette.FillDark);
            SetColor(dictionary, "BaoBao_Color_Header_Light", palette.HeaderLight);
            SetColor(dictionary, "BaoBao_Color_Header_Dark", palette.HeaderDark);

            SetColor(dictionary, "BaoBao_Color_Text_Primary_Light", palette.TextPrimaryLight);
            SetColor(dictionary, "BaoBao_Color_Text_Primary_Dark", palette.TextPrimaryDark);
            SetColor(dictionary, "BaoBao_Color_Text_Secondary_Light", palette.TextSecondaryLight);
            SetColor(dictionary, "BaoBao_Color_Text_Secondary_Dark", palette.TextSecondaryDark);
            SetColor(dictionary, "BaoBao_Color_Text_PlaceHolder_Light", palette.TextPlaceHolderLight);
            SetColor(dictionary, "BaoBao_Color_Text_PlaceHolder_Dark", palette.TextPlaceHolderDark);

            SetColor(dictionary, "BaoBao_Color_Border_Light", palette.BorderLight);
            SetColor(dictionary, "BaoBao_Color_Border_Dark", palette.BorderDark);

            SetBrush(dictionary, "BaoBao_Primary_Brush", "BaoBao_Color_Primary");
            SetBrush(dictionary, "BaoBao_Primary_Hover_Brush", "BaoBao_Color_Primary_Hover");
            SetBrush(dictionary, "BaoBao_Primary_Pressed_Brush", "BaoBao_Color_Primary_Pressed");
            SetBrush(dictionary, "BaoBao_Success_Brush", "BaoBao_Color_Success");
            SetBrush(dictionary, "BaoBao_Info_Brush", "BaoBao_Color_Info");
            SetBrush(dictionary, "BaoBao_Warning_Brush", "BaoBao_Color_Warning");
            SetBrush(dictionary, "BaoBao_Error_Brush", "BaoBao_Color_Error");

            return dictionary;
        }

        private static void SetColor(ResourceDictionary dictionary, string key, Color? color)
        {
            if (color.HasValue)
            {
                dictionary[key] = color.Value;
            }
        }

        private static void SetBrush(ResourceDictionary dictionary, string brushKey, string colorKey)
        {
            dictionary[brushKey] = new SolidColorBrush((Color)dictionary[colorKey]);
        }

        private static void ReapplyCurrentPalette()
        {
            if (CurrentPaletteMode == BaoBaoPaletteMode.Custom && CurrentCustomPalette != null)
            {
                SetCustomPalette(CurrentCustomPalette);
                return;
            }

            SetPalette(CurrentBuiltInPalette ?? BaoBaoPalette.Default);
        }

        private static BaoBaoPalette? StringToPalette(string paletteName)
        {
            if (string.IsNullOrWhiteSpace(paletteName))
            {
                return BaoBaoPalette.Default;
            }

            foreach (BaoBaoPalette palette in Enum.GetValues(typeof(BaoBaoPalette)))
            {
                if (string.Equals(PaletteToString(palette), paletteName, StringComparison.OrdinalIgnoreCase))
                {
                    return palette;
                }
            }

            return null;
        }

        private static BaoBaoCustomPalette ClonePalette(BaoBaoCustomPalette palette)
        {
            return new BaoBaoCustomPalette
            {
                Primary = palette.Primary,
                PrimaryHover = palette.PrimaryHover,
                PrimaryPressed = palette.PrimaryPressed,
                Success = palette.Success,
                Info = palette.Info,
                Warning = palette.Warning,
                Error = palette.Error,
                BackgroundLight = palette.BackgroundLight,
                BackgroundDark = palette.BackgroundDark,
                SurfaceLight = palette.SurfaceLight,
                SurfaceDark = palette.SurfaceDark,
                FillLight = palette.FillLight,
                FillDark = palette.FillDark,
                HeaderLight = palette.HeaderLight,
                HeaderDark = palette.HeaderDark,
                TextPrimaryLight = palette.TextPrimaryLight,
                TextPrimaryDark = palette.TextPrimaryDark,
                TextSecondaryLight = palette.TextSecondaryLight,
                TextSecondaryDark = palette.TextSecondaryDark,
                TextPlaceHolderLight = palette.TextPlaceHolderLight,
                TextPlaceHolderDark = palette.TextPlaceHolderDark,
                BorderLight = palette.BorderLight,
                BorderDark = palette.BorderDark
            };
        }

        internal static string PaletteToString(BaoBaoPalette palette)
        {
            return palette switch
            {
                BaoBaoPalette.Default => "Default",
                BaoBaoPalette.Lime => "Lime",
                BaoBaoPalette.Blue => "Blue",
                BaoBaoPalette.Red => "Red",
                BaoBaoPalette.Purple => "Purple",
                BaoBaoPalette.Orange => "Orange",
                BaoBaoPalette.Cyan => "Cyan",
                BaoBaoPalette.Github => "Github",
                BaoBaoPalette.VisualStudio => "VisualStudio",
                _ => "Default"
            };
        }
    }
}
