using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BaoBao.UI.WPF.Controls;
using BaoBao.UI.WPF.Helpers;

namespace BaoBao.UI.WPF.Demo
{
    public class UserData
    {
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }

    public partial class MainWindow : BaoBaoWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var users = new List<UserData>
            {
                new UserData { ID = "001", Name = "Alice", Status = "Active", Date = "2023-10-01" },
                new UserData { ID = "002", Name = "Bob", Status = "Inactive", Date = "2023-10-02" },
                new UserData { ID = "003", Name = "Charlie", Status = "Active", Date = "2023-10-05" },
                new UserData { ID = "004", Name = "David", Status = "Pending", Date = "2023-10-08" },
            };

            UserDataGrid.ItemsSource = users;
            DashboardDataGrid.ItemsSource = users;

            OrderTags.Tags = new ObservableCollection<string> { "Urgent", "Fragile" };

            InitializeIcons();
            IconList.ItemsSource = FilteredIcons;

            PaletteSelector.ItemsSource = Enum.GetValues(typeof(BaoBaoPalette));
            PaletteSelector.SelectedItem = BaoBaoPalette.Default;
            CustomPaletteHexTextBox.Text = "#84cc16";
            UpdatePaletteStatus();
        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            BaoBaoToast.Info("This is an info message.");
        }

        private void BtnSuccess_Click(object sender, RoutedEventArgs e)
        {
            BaoBaoToast.Success("Operation completed successfully!");
        }

        private void BtnWarn_Click(object sender, RoutedEventArgs e)
        {
            BaoBaoToast.Warning("Please check your input carefully.");
        }

        private void BtnError_Click(object sender, RoutedEventArgs e)
        {
            BaoBaoToast.Error("An unexpected error occurred.");
        }

        private void BtnDialog_Click(object sender, RoutedEventArgs e)
        {
            var result = BaoBaoMessageBox.Show("Do you like this new UI framework?", "Feedback", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                BaoBaoToast.Success("Thanks! We appreciate it.");
            }
            else if (result == MessageBoxResult.No)
            {
                BaoBaoToast.Warning("We will try harder next time.");
            }
            else
            {
                BaoBaoToast.Info("You cancelled the dialog.");
            }
        }

        private void BtnOpenBaoBaoDrawer_Click(object sender, RoutedEventArgs e)
        {
            SettingsBaoBaoDrawer.IsOpen = true;
        }

        private async void BtnBaoBaoLoading_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await StartBaoBaoLoading(async token =>
                {
                    await System.Threading.Tasks.Task.Delay(3000, token);
                    BaoBaoToast.Success("BaoBaoLoading Completed!");
                }, "Processing...", true);
            }
            catch (OperationCanceledException)
            {
                BaoBaoToast.Warning("BaoBaoLoading Cancelled!");
            }
        }

        private async void BtnLoadSimple_Click(object sender, RoutedEventArgs e)
        {
            await StartBaoBaoLoading(async () => await System.Threading.Tasks.Task.Delay(2000), "Simple BaoBaoLoading...");
            BaoBaoToast.Success("Done!");
        }

        private async void BtnLoadCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await StartBaoBaoLoading(async ct => await System.Threading.Tasks.Task.Delay(5000, ct), "Long Task (Click X to cancel)", true);
                BaoBaoToast.Success("Long task finished.");
            }
            catch (OperationCanceledException)
            {
                BaoBaoToast.Info("Task was cancelled.");
            }
        }

        private async void BtnLoadResult_Click(object sender, RoutedEventArgs e)
        {
            var result = await StartBaoBaoLoading(async () =>
            {
                await System.Threading.Tasks.Task.Delay(1500);
                return 42;
            }, "Calculating...");

            BaoBaoToast.Info($"Calculation Result: {result}");
        }

        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.SetTheme(ThemeType.Dark);
            UpdatePaletteStatus();
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ThemeManager.SetTheme(ThemeType.Light);
            UpdatePaletteStatus();
        }

        private void PaletteSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaletteSelector.SelectedItem is BaoBaoPalette palette)
            {
                ThemeManager.SetPalette(palette);
                UpdatePaletteStatus();
            }
        }

        private void ApplyCustomPalette_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hex = CustomPaletteHexTextBox.Text?.Trim() ?? string.Empty;
                ThemeManager.SetPaletteFromHex(hex);
                PaletteSelector.SelectedItem = null;
                UpdatePaletteStatus();
                BaoBaoToast.Success($"Applied custom brand color {hex}.");
            }
            catch (Exception)
            {
                BaoBaoToast.Error("Invalid color. Try a hex value like #84cc16.");
            }
        }

        private void ResetPalette_Click(object sender, RoutedEventArgs e)
        {
            PaletteSelector.SelectedItem = BaoBaoPalette.Default;
            ThemeManager.SetPalette(BaoBaoPalette.Default);
            UpdatePaletteStatus();
            BaoBaoToast.Info("Reverted to the default palette.");
        }

        private void UpdatePaletteStatus()
        {
            string theme = ThemeManager.CurrentTheme.ToString();

            if (ThemeManager.CurrentPaletteMode == BaoBaoPaletteMode.Custom)
            {
                var primary = ThemeManager.CurrentCustomPalette?.Primary ?? Colors.Transparent;
                CustomPaletteHexTextBox.Text = primary == Colors.Transparent ? CustomPaletteHexTextBox.Text : primary.ToString();
                PaletteStatusText.Text = $"{theme} | Custom";
                return;
            }

            var palette = ThemeManager.CurrentBuiltInPalette ?? BaoBaoPalette.Default;
            PaletteStatusText.Text = $"{theme} | {palette}";
        }
    }
}
