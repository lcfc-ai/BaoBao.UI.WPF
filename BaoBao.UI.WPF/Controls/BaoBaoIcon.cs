using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace BaoBao.UI.WPF.Controls
{
    /// <summary>
    /// Wrapper for PackIconMaterial to ensure consistent styling across the application.
    /// </summary>
    public class BaoBaoIcon : Control
    {
        static BaoBaoIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoIcon), new FrameworkPropertyMetadata(typeof(BaoBaoIcon)));
        }



        public PackIconMaterialKind Icon
        {
            get { return (PackIconMaterialKind)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(PackIconMaterialKind), typeof(BaoBaoIcon), new PropertyMetadata(null));


    }
}

