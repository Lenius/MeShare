using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            var _viewModel = DataContext as MainWindowViewModel;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
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
                        _viewModel.Add(new S3Object() { FileName = file });
                    }


                }

            }
        }

        private void CommandManager_OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == DataGrid.DeleteCommand)
            {
                if (MessageBox.Show("Are you sure you want to delete?", "Please confirm.", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No) != MessageBoxResult.Yes)
                {
                    // Cancel Delete.
                    e.Handled = true;
                }
            }
        }
    }
}
