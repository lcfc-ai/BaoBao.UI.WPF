using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BaoBao.UI.WPF.Controls
{
    public class BaoBaoAvatar : Control
    {
        static BaoBaoAvatar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoAvatar), new FrameworkPropertyMetadata(typeof(BaoBaoAvatar)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // Listen for Image Failed
            if (GetTemplateChild("PART_Image") is Image img)
            {
                img.ImageFailed += (s, e) =>
                {
                    // Hide image, show icon/text fallback
                    img.Visibility = Visibility.Collapsed;
                };
            }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(BaoBaoAvatar), new PropertyMetadata(null));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(BaoBaoAvatar), new PropertyMetadata(null));

        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BaoBaoAvatar), new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BaoBaoAvatar), new PropertyMetadata(new CornerRadius(50))); // Default circle-ish

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
    }
}
