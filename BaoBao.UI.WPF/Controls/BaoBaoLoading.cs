using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BaoBao.UI.WPF.Controls
{
    [TemplatePart(Name = "PART_CancelButton", Type = typeof(Button))]
    public class BaoBaoLoading : ContentControl
    {
        static BaoBaoLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoLoading), new FrameworkPropertyMetadata(typeof(BaoBaoLoading)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("PART_CancelButton") is Button btn)
            {
                btn.Click += (s, e) =>
                {
                    RaiseEvent(new RoutedEventArgs(CancelEvent));
                };
            }
        }

        public bool IsBaoBaoLoading
        {
            get { return (bool)GetValue(IsBaoBaoLoadingProperty); }
            set { SetValue(IsBaoBaoLoadingProperty, value); }
        }
        public static readonly DependencyProperty IsBaoBaoLoadingProperty =
            DependencyProperty.Register("IsBaoBaoLoading", typeof(bool), typeof(BaoBaoLoading), new PropertyMetadata(false));

        public string BaoBaoLoadingText
        {
            get { return (string)GetValue(BaoBaoLoadingTextProperty); }
            set { SetValue(BaoBaoLoadingTextProperty, value); }
        }
        public static readonly DependencyProperty BaoBaoLoadingTextProperty =
            DependencyProperty.Register("BaoBaoLoadingText", typeof(string), typeof(BaoBaoLoading), new PropertyMetadata("BaoBaoLoading..."));

        public bool IsCancellable
        {
            get { return (bool)GetValue(IsCancellableProperty); }
            set { SetValue(IsCancellableProperty, value); }
        }
        public static readonly DependencyProperty IsCancellableProperty =
            DependencyProperty.Register("IsCancellable", typeof(bool), typeof(BaoBaoLoading), new PropertyMetadata(false));

        // 路由事件
        public static readonly RoutedEvent CancelEvent = EventManager.RegisterRoutedEvent(
            "Cancel", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BaoBaoLoading));

        public event RoutedEventHandler Cancel
        {
            add { AddHandler(CancelEvent, value); }
            remove { RemoveHandler(CancelEvent, value); }
        }
    }
}
