using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.Globalization;

namespace BaoBao.UI.WPF.Controls
{
    public class BaoBaoStepBar : ItemsControl
    {
        static BaoBaoStepBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoStepBar), new FrameworkPropertyMetadata(typeof(BaoBaoStepBar)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is BaoBaoStepBarItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BaoBaoStepBarItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is BaoBaoStepBarItem stepItem)
            {
                 // Determine index
                 int index = this.ItemContainerGenerator.IndexFromContainer(element);
                 stepItem.Index = index + 1;
                 stepItem.IsFirst = index == 0;
                 stepItem.IsLast = index == this.Items.Count - 1;
                 stepItem.Click += StepItem_Click;
                 UpdateItemState(stepItem);
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            if (element is BaoBaoStepBarItem stepItem)
            {
                stepItem.Click -= StepItem_Click;
            }
        }

        private void StepItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is BaoBaoStepBarItem item)
            {
                StepIndex = item.Index - 1;
            }
        }


        public int StepIndex
        {
            get { return (int)GetValue(StepIndexProperty); }
            set { SetValue(StepIndexProperty, value); }
        }
        public static readonly DependencyProperty StepIndexProperty =
            DependencyProperty.Register("StepIndex", typeof(int), typeof(BaoBaoStepBar), 
                new PropertyMetadata(0, OnStepIndexChanged));

        private static void OnStepIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BaoBaoStepBar)d).UpdateAllItems();
        }

        private void UpdateAllItems()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                var container = this.ItemContainerGenerator.ContainerFromIndex(i) as BaoBaoStepBarItem;
                if (container != null)
                {
                    container.IsFirst = (i == 0);
                    container.IsLast = (i == this.Items.Count - 1);
                    UpdateItemState(container);
                }
            }
        }

        private void UpdateItemState(BaoBaoStepBarItem item)
        {
            int index = this.ItemContainerGenerator.IndexFromContainer(item);
            item.Index = index + 1;
            
            if (index < StepIndex)
            {
                item.Status = StepStatus.Complete;
            }
            else if (index == StepIndex)
            {
                item.Status = StepStatus.Active;
            }
            else
            {
                item.Status = StepStatus.Pending;
            }
        }
    }

    public class BaoBaoStepBarItem : ContentControl
    {
        static BaoBaoStepBarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoStepBarItem), new FrameworkPropertyMetadata(typeof(BaoBaoStepBarItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // Attach click handler if template has a button or we make the whole control clickable
            // For simplicity, let's handle MouseLeftButtonDown on the control itself
            this.MouseLeftButtonDown += (s, e) => RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BaoBaoStepBarItem));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(BaoBaoStepBarItem), new PropertyMetadata(0));

        public bool IsLast
        {
            get { return (bool)GetValue(IsLastProperty); }
            set { SetValue(IsLastProperty, value); }
        }
        public static readonly DependencyProperty IsLastProperty =
            DependencyProperty.Register("IsLast", typeof(bool), typeof(BaoBaoStepBarItem), new PropertyMetadata(false));

        public bool IsFirst
        {
            get { return (bool)GetValue(IsFirstProperty); }
            set { SetValue(IsFirstProperty, value); }
        }
        public static readonly DependencyProperty IsFirstProperty =
            DependencyProperty.Register("IsFirst", typeof(bool), typeof(BaoBaoStepBarItem), new PropertyMetadata(false));

        public StepStatus Status
        {
            get { return (StepStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(StepStatus), typeof(BaoBaoStepBarItem), new PropertyMetadata(StepStatus.Pending));
    }

    public enum StepStatus
    {
        Pending,
        Active,
        Complete
    }
}
