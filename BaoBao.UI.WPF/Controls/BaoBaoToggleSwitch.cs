using System.Windows;
using System.Windows.Controls.Primitives;

namespace BaoBao.UI.WPF.Controls
{
    public class BaoBaoToggleSwitch : ToggleButton
    {
        static BaoBaoToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoToggleSwitch), new FrameworkPropertyMetadata(typeof(BaoBaoToggleSwitch)));
        }

        public static readonly DependencyProperty OnContentProperty =
            DependencyProperty.Register("OnContent", typeof(object), typeof(BaoBaoToggleSwitch), new PropertyMetadata(null));

        public object OnContent
        {
            get { return GetValue(OnContentProperty); }
            set { SetValue(OnContentProperty, value); }
        }

        public static readonly DependencyProperty OffContentProperty =
            DependencyProperty.Register("OffContent", typeof(object), typeof(BaoBaoToggleSwitch), new PropertyMetadata(null));

        public object OffContent
        {
            get { return GetValue(OffContentProperty); }
            set { SetValue(OffContentProperty, value); }
        }
    }
}

