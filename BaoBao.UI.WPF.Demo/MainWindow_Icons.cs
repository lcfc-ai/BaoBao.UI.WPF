using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BaoBao.UI.WPF.Controls;
using BaoBao.UI.WPF.Helpers;
using MahApps.Metro.IconPacks;

namespace BaoBao.UI.WPF.Demo
{
    public partial class MainWindow
    {
        // Add new region for Icon handling
        #region Icon Gallery

        public ObservableCollection<string> AllIcons { get; set; } = new();
        public ObservableCollection<string> FilteredIcons { get; set; } = new();
        private int _currentPage = 1;
        private int _pageSize = 100;

        private void InitializeIcons()
        {
            AllIcons = new ObservableCollection<string>(IconHelper.GetAllMaterialIconNames());
            FilteredIcons = new ObservableCollection<string>();
            
            // Set DataContext for icon binding if not already set or needs specific handling
            // Assuming MainWindow DataContext is self or viewmodel. 
            // For this demo, let's just set ItemsSource in code behind or ensure property is accessible.
            
            UpdateIconPage();
        }

        private void UpdateIconPage()
        {
            if (AllIcons == null) return;

            var paged = AllIcons.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();
            FilteredIcons.Clear();
            foreach (var icon in paged)
            {
                FilteredIcons.Add(icon);
            }
            
            if (IconBaoBaoPagination != null)
            {
                IconBaoBaoPagination.TotalCount = AllIcons.Count;
                IconBaoBaoPagination.CurrentPage = _currentPage;
                IconBaoBaoPagination.PageSize = _pageSize;
            }
        }

        private void IconSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            string query = textBox?.Text?.ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(query))
            {
                 AllIcons = new ObservableCollection<string>(IconHelper.GetAllMaterialIconNames());
            }
            else
            {
                 var filtered = IconHelper.GetAllMaterialIconNames()
                    .Where(x => x.ToLower().Contains(query))
                    .ToList();
                 AllIcons = new ObservableCollection<string>(filtered);
            }
            
            _currentPage = 1;
            UpdateIconPage();
        }

        private void IconBaoBaoPagination_PageChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            _currentPage = e.NewValue;
            UpdateIconPage();
        }

        private void IconItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is string iconName)
            {
                Clipboard.SetText($"<iconPacks:PackIconMaterial Kind=\"{iconName}\" />");
                BaoBaoToast.Success($"Copied to clipboard: {iconName}");
            }
        }

        #endregion
    }
}

