using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BaoBao.UI.WPF.Controls
{
    public class BaoBaoSkeleton : Control
    {
        static BaoBaoSkeleton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoSkeleton), new FrameworkPropertyMetadata(typeof(BaoBaoSkeleton)));
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(BaoBaoSkeleton), new PropertyMetadata(true));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BaoBaoSkeleton), new PropertyMetadata(new CornerRadius(4)));
    }
}

