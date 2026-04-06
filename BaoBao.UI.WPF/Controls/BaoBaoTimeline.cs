using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BaoBao.UI.WPF.Controls
{
    public enum BaoBaoTimelineLineType
    {
        Solid,
        Dashed,
        Dotted
    }

    public class BaoBaoTimeline : ItemsControl
    {
        static BaoBaoTimeline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoTimeline), new FrameworkPropertyMetadata(typeof(BaoBaoTimeline)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is BaoBaoTimelineItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BaoBaoTimelineItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            UpdateItem(element as BaoBaoTimelineItem);
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            for (int i = 0; i < this.Items.Count; i++)
            {
                var container = this.ItemContainerGenerator.ContainerFromIndex(i) as BaoBaoTimelineItem;
                if (container != null)
                {
                    UpdateItem(container);
                }
            }
        }

        private void UpdateItem(BaoBaoTimelineItem? item)
        {
            if (item == null) return;
            int index = this.ItemContainerGenerator.IndexFromContainer(item);
            item.IsLast = index == this.Items.Count - 1;
        }
    }

    public class BaoBaoTimelineItem : ContentControl
    {
        static BaoBaoTimelineItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoTimelineItem), new FrameworkPropertyMetadata(typeof(BaoBaoTimelineItem)));
        }

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(BaoBaoTimelineItem), new PropertyMetadata(null));

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(BaoBaoTimelineItem), new PropertyMetadata(null));

        public BaoBaoTimelineLineType LineType
        {
            get { return (BaoBaoTimelineLineType)GetValue(LineTypeProperty); }
            set { SetValue(LineTypeProperty, value); }
        }
        public static readonly DependencyProperty LineTypeProperty =
            DependencyProperty.Register("LineType", typeof(BaoBaoTimelineLineType), typeof(BaoBaoTimelineItem), new PropertyMetadata(BaoBaoTimelineLineType.Solid));

        public bool IsLast
        {
            get { return (bool)GetValue(IsLastProperty); }
            set { SetValue(IsLastProperty, value); }
        }
        public static readonly DependencyProperty IsLastProperty =
            DependencyProperty.Register("IsLast", typeof(bool), typeof(BaoBaoTimelineItem), new PropertyMetadata(false));
    }
}
