using System.Windows;

namespace BaoBao.UI.WPF.Controls
{
    public class BaoBaoMessageBox : BaoBaoWindow
    {
        static BaoBaoMessageBox()
        {
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(BaoBaoMessageBox), new FrameworkPropertyMetadata(typeof(BaoBaoMessageBox)));
        }

        public static MessageBoxResult Show(string messageBoxText, string caption = "Message", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            var msgBox = new BaoBaoMessageBox
            {
                Title = caption,
                MessageText = messageBoxText,
                BoxButton = button,
                BoxImage = icon,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,  
            };
            
            // Try to owner
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                msgBox.Owner = Application.Current.MainWindow;
                msgBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            msgBox.ShowDialog();
            return msgBox.Result;
        }

        public string MessageText
        {
            get { return (string)GetValue(MessageTextProperty); }
            set { SetValue(MessageTextProperty, value); }
        }
        public static readonly DependencyProperty MessageTextProperty = DependencyProperty.Register("MessageText", typeof(string), typeof(BaoBaoMessageBox), new PropertyMetadata(string.Empty));

        public MessageBoxButton BoxButton
        {
            get { return (MessageBoxButton)GetValue(BoxButtonProperty); }
            set { SetValue(BoxButtonProperty, value); }
        }
        public static readonly DependencyProperty BoxButtonProperty = DependencyProperty.Register("BoxButton", typeof(MessageBoxButton), typeof(BaoBaoMessageBox), new PropertyMetadata(MessageBoxButton.OK));

        public MessageBoxImage BoxImage
        {
            get { return (MessageBoxImage)GetValue(BoxImageProperty); }
            set { SetValue(BoxImageProperty, value); }
        }
        public static readonly DependencyProperty BoxImageProperty = DependencyProperty.Register("BoxImage", typeof(MessageBoxImage), typeof(BaoBaoMessageBox), new PropertyMetadata(MessageBoxImage.None));

        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("PART_BtnOK") is System.Windows.Controls.Button btnOK)
                btnOK.Click += (s, e) => { Result = MessageBoxResult.OK; Close(); };
            if (GetTemplateChild("PART_BtnYes") is System.Windows.Controls.Button btnYes)
                btnYes.Click += (s, e) => { Result = MessageBoxResult.Yes; Close(); };
            if (GetTemplateChild("PART_BtnNo") is System.Windows.Controls.Button btnNo)
                btnNo.Click += (s, e) => { Result = MessageBoxResult.No; Close(); };
            if (GetTemplateChild("PART_BtnCancel") is System.Windows.Controls.Button btnCancel)
                btnCancel.Click += (s, e) => { Result = MessageBoxResult.Cancel; Close(); };
        }
    }
}

