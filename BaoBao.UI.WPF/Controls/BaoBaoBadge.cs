using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BaoBao.UI.WPF.Controls
{
    public class BaoBaoBadge : ContentControl
    {
        static BaoBaoBadge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoBadge), new FrameworkPropertyMetadata(typeof(BaoBaoBadge)));
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(BaoBaoBadge), new PropertyMetadata(null, OnValueChanged));

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(BaoBaoBadge), new PropertyMetadata(99, OnValueChanged));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty IsDotProperty =
            DependencyProperty.Register("IsDot", typeof(bool), typeof(BaoBaoBadge), new PropertyMetadata(false));

        public bool IsDot
        {
            get { return (bool)GetValue(IsDotProperty); }
            set { SetValue(IsDotProperty, value); }
        }

        public static readonly DependencyProperty ShowZeroProperty =
            DependencyProperty.Register("ShowZero", typeof(bool), typeof(BaoBaoBadge), new PropertyMetadata(false, OnValueChanged));

        public bool ShowZero
        {
            get { return (bool)GetValue(ShowZeroProperty); }
            set { SetValue(ShowZeroProperty, value); }
        }
        
        public static readonly DependencyProperty BaoBaoBadgeBackgroundProperty =
            DependencyProperty.Register("BaoBaoBadgeBackground", typeof(System.Windows.Media.Brush), typeof(BaoBaoBadge), new PropertyMetadata(null));

        public System.Windows.Media.Brush BaoBaoBadgeBackground
        {
            get { return (System.Windows.Media.Brush)GetValue(BaoBaoBadgeBackgroundProperty); }
            set { SetValue(BaoBaoBadgeBackgroundProperty, value); }
        }
        
        // Offset Property
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(Point), typeof(BaoBaoBadge), new PropertyMetadata(new Point(0, 0)));

        public Point Offset
        {
            get { return (Point)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        private static readonly DependencyPropertyKey BaoBaoBadgeTextPropertyKey =
            DependencyProperty.RegisterReadOnly("BaoBaoBadgeText", typeof(string), typeof(BaoBaoBadge), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty BaoBaoBadgeTextProperty = BaoBaoBadgeTextPropertyKey.DependencyProperty;

        public string BaoBaoBadgeText
        {
            get { return (string)GetValue(BaoBaoBadgeTextProperty); }
            private set { SetValue(BaoBaoBadgeTextPropertyKey, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BaoBaoBadge)?.UpdateBaoBaoBadgeText();
        }

        private void UpdateBaoBaoBadgeText()
        {
            if (Value is int i)
            {
                if (i == 0 && !ShowZero) 
                    BaoBaoBadgeText = string.Empty;
                else if (i > Maximum) 
                    BaoBaoBadgeText = $"{Maximum}+";
                else 
                    BaoBaoBadgeText = i.ToString();
            }
            else
            {
                BaoBaoBadgeText = Value?.ToString();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateBaoBaoBadgeText();
        }
    }
}
