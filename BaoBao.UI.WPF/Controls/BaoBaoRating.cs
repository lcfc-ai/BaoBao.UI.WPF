using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace BaoBao.UI.WPF.Controls
{
    [TemplatePart(Name = ElementStarsPanel, Type = typeof(Panel))]
    public class BaoBaoRating : Control
    {
        private const string ElementStarsPanel = "PART_StarsPanel";
        private Panel? _starsPanel;

        static BaoBaoRating()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoRating), new FrameworkPropertyMetadata(typeof(BaoBaoRating)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _starsPanel = GetTemplateChild(ElementStarsPanel) as Panel;
            GenerateStars();
            UpdateStars(Value);
        }

        #region Properties

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(BaoBaoRating),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (BaoBaoRating)d;
            control.UpdateStars((double)e.NewValue);
            control.RaiseValueChangedEvent((double)e.OldValue, (double)e.NewValue);
        }

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(BaoBaoRating),
                new PropertyMetadata(5, OnMaximumChanged));

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BaoBaoRating)d).GenerateStars();
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(BaoBaoRating), new PropertyMetadata(false));

        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }

        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(double), typeof(BaoBaoRating), new PropertyMetadata(24.0));

        public bool AllowHalf
        {
            get { return (bool)GetValue(AllowHalfProperty); }
            set { SetValue(AllowHalfProperty, value); }
        }

        public static readonly DependencyProperty AllowHalfProperty =
            DependencyProperty.Register("AllowHalf", typeof(bool), typeof(BaoBaoRating), new PropertyMetadata(false));

        #endregion

        #region Events

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(BaoBaoRating));

        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private void RaiseValueChangedEvent(double oldValue, double newValue)
        {
            RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldValue, newValue, ValueChangedEvent));
        }

        #endregion

        private void GenerateStars()
        {
            if (_starsPanel == null) return;

            _starsPanel.Children.Clear();
            for (int i = 1; i <= Maximum; i++)
            {
                var item = new BaoBaoRatingItem
                {
                    Value = i,
                    Width = ItemSize,
                    Height = ItemSize
                };

                var binding = new System.Windows.Data.Binding("ItemSize") { Source = this };
                item.SetBinding(WidthProperty, binding);
                item.SetBinding(HeightProperty, binding);

                // Pass mouse events to parent logic
                item.MouseMove += Item_MouseMove;
                item.MouseLeave += Item_MouseLeave;
                item.Click += Item_Click;

                _starsPanel.Children.Add(item);
            }
            UpdateStars(Value);
        }

        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsReadOnly) return;
            if (sender is BaoBaoRatingItem item)
            {
                double val = item.Value;
                if (AllowHalf)
                {
                    Point p = e.GetPosition(item);
                    if (p.X < item.ActualWidth / 2)
                    {
                        val -= 0.5;
                    }
                }
                UpdateStars(val);
            }
        }

        private void Item_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsReadOnly) return;
            // Only reset if we leave the whole control, handled by parent OnMouseLeave
        }
        
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!IsReadOnly)
            {
                UpdateStars(Value);
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly) return;
            if (sender is BaoBaoRatingItem item)
            {
                double val = item.Value;
                if (AllowHalf)
                {
                    // We need the mouse position relative to the item to determine half
                    // But Click event doesn't give mouse pos easily without args.
                    // We can use Mouse.GetPosition
                    Point p = Mouse.GetPosition(item);
                    if (p.X < item.ActualWidth / 2)
                    {
                        val -= 0.5;
                    }
                }
                Value = val;
            }
        }

        private void UpdateStars(double value)
        {
            if (_starsPanel == null) return;
            foreach (UIElement child in _starsPanel.Children)
            {
                if (child is BaoBaoRatingItem item)
                {
                    if (value >= item.Value)
                    {
                        item.State = BaoBaoRatingItemState.Full;
                    }
                    else if (value >= item.Value - 0.5)
                    {
                        item.State = BaoBaoRatingItemState.Half;
                    }
                    else
                    {
                        item.State = BaoBaoRatingItemState.BaoBaoEmpty;
                    }
                }
            }
        }
    }

    public class BaoBaoRatingItem : ButtonBase
    {
        static BaoBaoRatingItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoRatingItem), new FrameworkPropertyMetadata(typeof(BaoBaoRatingItem)));
        }

        public int Value { get; set; }

        public BaoBaoRatingItemState State
        {
            get { return (BaoBaoRatingItemState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(BaoBaoRatingItemState), typeof(BaoBaoRatingItem), new PropertyMetadata(BaoBaoRatingItemState.BaoBaoEmpty));
    }

    public enum BaoBaoRatingItemState
    {
        BaoBaoEmpty,
        Half,
        Full
    }
}
