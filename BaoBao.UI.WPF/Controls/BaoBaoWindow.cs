using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;

namespace BaoBao.UI.WPF.Controls
{
    [TemplatePart(Name = "PART_MessageContainer", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_WindowBaoBaoLoading", Type = typeof(BaoBaoLoading))]
    public class BaoBaoWindow : Window
    {
        static BaoBaoWindow()
        {
            // Use Implicit Style from App.Resources instead of DefaultStyleKey to ensure stability
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoWindow), new FrameworkPropertyMetadata(typeof(BaoBaoWindow)));
        }

        public BaoBaoWindow()
        {
            // Default setup
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));

            // Enable System Rounded Corners (Win11+)
            Helpers.WindowRoundedCornerHelper.EnableRoundedCorners(this);
        }

        #region Dependency Properties

        // IsBaoBaoLoading
        public static readonly DependencyProperty IsBaoBaoLoadingProperty =
            DependencyProperty.Register("IsBaoBaoLoading", typeof(bool), typeof(BaoBaoWindow), new PropertyMetadata(false));

        public bool IsBaoBaoLoading
        {
            get { return (bool)GetValue(IsBaoBaoLoadingProperty); }
            set { SetValue(IsBaoBaoLoadingProperty, value); }
        }

        // BaoBaoLoadingText
        public static readonly DependencyProperty BaoBaoLoadingTextProperty =
            DependencyProperty.Register("BaoBaoLoadingText", typeof(string), typeof(BaoBaoWindow), new PropertyMetadata("BaoBaoLoading..."));

        public string BaoBaoLoadingText
        {
            get { return (string)GetValue(BaoBaoLoadingTextProperty); }
            set { SetValue(BaoBaoLoadingTextProperty, value); }
        }

        // IsBaoBaoLoadingCancellable
        public static readonly DependencyProperty IsBaoBaoLoadingCancellableProperty =
            DependencyProperty.Register("IsBaoBaoLoadingCancellable", typeof(bool), typeof(BaoBaoWindow), new PropertyMetadata(false));

        public bool IsBaoBaoLoadingCancellable
        {
            get { return (bool)GetValue(IsBaoBaoLoadingCancellableProperty); }
            set { SetValue(IsBaoBaoLoadingCancellableProperty, value); }
        }

        // TitleBarContent
        public static readonly DependencyProperty TitleBarContentProperty =
            DependencyProperty.Register("TitleBarContent", typeof(object), typeof(BaoBaoWindow), new PropertyMetadata(null));

        public object TitleBarContent
        {
            get { return GetValue(TitleBarContentProperty); }
            set { SetValue(TitleBarContentProperty, value); }
        }

        // ExtendViewIntoTitleBar
        public static readonly DependencyProperty ExtendViewIntoTitleBarProperty =
            DependencyProperty.Register("ExtendViewIntoTitleBar", typeof(bool), typeof(BaoBaoWindow), new PropertyMetadata(false));

        public bool ExtendViewIntoTitleBar
        {
            get { return (bool)GetValue(ExtendViewIntoTitleBarProperty); }
            set { SetValue(ExtendViewIntoTitleBarProperty, value); }
        }

        // TitleBarHeight
        public static readonly DependencyProperty TitleBarHeightProperty =
            DependencyProperty.Register("TitleBarHeight", typeof(double), typeof(BaoBaoWindow), new PropertyMetadata(32.0));

        public double TitleBarHeight
        {
            get { return (double)GetValue(TitleBarHeightProperty); }
            set { SetValue(TitleBarHeightProperty, value); }
        }

        // ShowIcon
        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.Register("ShowIcon", typeof(bool), typeof(BaoBaoWindow), new PropertyMetadata(true));

        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

        // ShowTitle
        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register("ShowTitle", typeof(bool), typeof(BaoBaoWindow), new PropertyMetadata(true));

        public bool ShowTitle
        {
            get { return (bool)GetValue(ShowTitleProperty); }
            set { SetValue(ShowTitleProperty, value); }
        }

        // TitleBarBackground
        public static readonly DependencyProperty TitleBarBackgroundProperty =
            DependencyProperty.Register("TitleBarBackground", typeof(System.Windows.Media.Brush), typeof(BaoBaoWindow), new PropertyMetadata(null));

        public System.Windows.Media.Brush TitleBarBackground
        {
            get { return (System.Windows.Media.Brush)GetValue(TitleBarBackgroundProperty); }
            set { SetValue(TitleBarBackgroundProperty, value); }
        }

        #endregion

        #region Events

        public static readonly RoutedEvent BaoBaoLoadingCancelledEvent = EventManager.RegisterRoutedEvent(
            "BaoBaoLoadingCancelled", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BaoBaoWindow));

        public event RoutedEventHandler BaoBaoLoadingCancelled
        {
            add { AddHandler(BaoBaoLoadingCancelledEvent, value); }
            remove { RemoveHandler(BaoBaoLoadingCancelledEvent, value); }
        }

        #endregion

        #region BaoBaoLoading Helpers

        private CancellationTokenSource _activeBaoBaoLoadingCts;

        /// <summary>
        /// Starts BaoBaoLoading, executes the async action, and stops BaoBaoLoading automatically.
        /// </summary>
        public async Task StartBaoBaoLoading(Func<Task> action, string text = "BaoBaoLoading...", bool cancellable = false)
        {
            await StartBaoBaoLoading(async (_) => await action(), text, cancellable);
        }

        /// <summary>
        /// Starts BaoBaoLoading, executes the async action with cancellation support, and stops BaoBaoLoading automatically.
        /// </summary>
        public async Task StartBaoBaoLoading(Func<CancellationToken, Task> action, string text = "BaoBaoLoading...", bool cancellable = false)
        {
            // Prevent concurrent BaoBaoLoading overrides if needed, or just overwrite.
            // For simplicity, we overwrite the current state.

            IsBaoBaoLoading = true;
            BaoBaoLoadingText = text;
            IsBaoBaoLoadingCancellable = cancellable;

            _activeBaoBaoLoadingCts = new CancellationTokenSource();

            // Handler to cancel the token when user clicks cancel
            RoutedEventHandler cancelHandler = (s, e) =>
            {
                _activeBaoBaoLoadingCts?.Cancel();
            };

            if (cancellable)
            {
                this.BaoBaoLoadingCancelled += cancelHandler;
            }

            try
            {
                await action(_activeBaoBaoLoadingCts.Token);
            }
            finally
            {
                if (cancellable)
                {
                    this.BaoBaoLoadingCancelled -= cancelHandler;
                }

                IsBaoBaoLoading = false;
                IsBaoBaoLoadingCancellable = false;
                _activeBaoBaoLoadingCts?.Dispose();
                _activeBaoBaoLoadingCts = null;
            }
        }

        /// <summary>
        /// Starts BaoBaoLoading, executes the async function returning a value, and stops BaoBaoLoading automatically.
        /// </summary>
        public async Task<T> StartBaoBaoLoading<T>(Func<Task<T>> action, string text = "BaoBaoLoading...", bool cancellable = false)
        {
            return await StartBaoBaoLoading(async (_) => await action(), text, cancellable);
        }

        /// <summary>
        /// Starts BaoBaoLoading, executes the async function returning a value with cancellation support, and stops BaoBaoLoading automatically.
        /// </summary>
        public async Task<T> StartBaoBaoLoading<T>(Func<CancellationToken, Task<T>> action, string text = "BaoBaoLoading...", bool cancellable = false)
        {
            IsBaoBaoLoading = true;
            BaoBaoLoadingText = text;
            IsBaoBaoLoadingCancellable = cancellable;

            _activeBaoBaoLoadingCts = new CancellationTokenSource();

            RoutedEventHandler cancelHandler = (s, e) =>
            {
                _activeBaoBaoLoadingCts?.Cancel();
            };

            if (cancellable)
            {
                this.BaoBaoLoadingCancelled += cancelHandler;
            }

            try
            {
                return await action(_activeBaoBaoLoadingCts.Token);
            }
            finally
            {
                if (cancellable)
                {
                    this.BaoBaoLoadingCancelled -= cancelHandler;
                }

                IsBaoBaoLoading = false;
                IsBaoBaoLoadingCancellable = false;
                _activeBaoBaoLoadingCts?.Dispose();
                _activeBaoBaoLoadingCts = null;
            }
        }

        #endregion

        #region Command Handlers

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _messageContainer = GetTemplateChild("PART_MessageContainer") as StackPanel;
            
            if (GetTemplateChild("PART_WindowBaoBaoLoading") is BaoBaoLoading BaoBaoLoading)
            {
                BaoBaoLoading.Cancel += (s, e) => RaiseEvent(new RoutedEventArgs(BaoBaoLoadingCancelledEvent));
            }
        }

        private StackPanel _messageContainer;

        public void ShowMessage(string content, BaoBaoMessageType type, TimeSpan duration)
        {
            if (_messageContainer == null) return;

            var message = new BaoBaoMessageItem
            {
                Content = content,
                Type = type,
                Duration = duration
            };

            _messageContainer.Children.Add(message);
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        #endregion
    }
}
