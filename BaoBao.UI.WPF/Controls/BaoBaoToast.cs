using System;
using System.Linq;
using System.Windows;

namespace BaoBao.UI.WPF.Controls
{
    public static class BaoBaoToast
    {
        public static void Show(string content, BaoBaoMessageType type = BaoBaoMessageType.Info)
        {
            Show(content, type, TimeSpan.FromSeconds(3));
        }

        public static void Show(string content, BaoBaoMessageType type, TimeSpan duration)
        {
            if (Application.Current == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = GetActiveWindow();
                if (window != null)
                {
                    window.ShowMessage(content, type, duration);
                }
            });
        }
        
        public static void Info(string content) => Show(content, BaoBaoMessageType.Info);
        public static void Success(string content) => Show(content, BaoBaoMessageType.Success);
        public static void Warning(string content) => Show(content, BaoBaoMessageType.Warning);
        public static void Error(string content) => Show(content, BaoBaoMessageType.Error);

        private static BaoBaoWindow GetActiveWindow()
        {
            var active = Application.Current.Windows.OfType<BaoBaoWindow>().SingleOrDefault(x => x.IsActive);
            if (active == null)
            {
                active = Application.Current.MainWindow as BaoBaoWindow;
            }
            if (active == null && Application.Current.Windows.Count > 0)
            {
                 active = Application.Current.Windows.OfType<BaoBaoWindow>().LastOrDefault();
            }
            return active;
        }
    }
}

