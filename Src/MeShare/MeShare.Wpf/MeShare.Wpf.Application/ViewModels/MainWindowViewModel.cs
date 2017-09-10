using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Mvvm;

namespace MeShare.Wpf.Application.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        private S3Object _selectedItem;
        public DelegateCommand<S3Object> OpenUrlCommand { get; set; }
        public DelegateCommand<S3Object> CancelUploadCommand { get; set; }

        public MainWindowViewModel()
        {
            S3Objects = new ObservableCollection<S3Object>();
            S3Objects.CollectionChanged += S3Objects_CollectionChanged;

            OpenUrlCommand = new DelegateCommand<S3Object>(OpenUrl, CanOpenUrl).ObservesProperty(() => SelectedItem);
            CancelUploadCommand = new DelegateCommand<S3Object>(CancelUpload, CanCancelUpload).ObservesProperty(() => SelectedItem);
        }

        private bool CanCancelUpload(S3Object arg)
        {
            bool result = arg?.Process < 100;
            return result;
        }

        private void CancelUpload(S3Object obj)
        {
            obj.CancelUpload();
        }

        private bool CanOpenUrl(S3Object arg)
        {
            return SelectedItem != null;
        }

        private void OpenUrl(S3Object obj)
        {
            var url = obj.GetUrl();
            System.Diagnostics.Process.Start(url);
        }

        private async void S3Objects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //add new items
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (S3Object newItem in e.NewItems)
                {
                    await newItem.Start();
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (S3Object oldItem in e.OldItems)
                {
                    oldItem.Dispose();
                }
            }
        }

        public ObservableCollection<S3Object> S3Objects { get; set; }

        public void Add(S3Object s3Object)
        {
            S3Objects.Add(s3Object);
        }

        public S3Object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }

        public void Dispose()
        {
            foreach (var viewModelS3Object in S3Objects)
            {
                viewModelS3Object.Dispose();
            }
        }
    }
}
