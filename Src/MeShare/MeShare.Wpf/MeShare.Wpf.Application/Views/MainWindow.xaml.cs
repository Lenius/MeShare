using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MeShare.Wpf.Application.ViewModels;

namespace MeShare.Wpf.Application.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            // Note that you can have more than one file.
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                var ext = info.Extension.ToLower();

                if (ext.Equals(".bmp") ||
                    ext.Equals(".gif") ||
                    ext.Equals(".jpg") ||
                    ext.Equals(".jpeg") ||
                    ext.Equals(".zip") ||
                    ext.Equals(".png"))
                {
                    viewModel?.Add(new S3Object() { FileName = file });
                }
            }
        }

        private void CommandManager_OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command != DataGrid.DeleteCommand) return;

            if (MessageBox.Show("Are you sure you want to remove?", "Please confirm.", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No) != MessageBoxResult.Yes)
            {
                // Cancel Delete.
                e.Handled = true;
            }
        }

        private void BtnExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            viewModel?.Dispose();
        }
    }
}
