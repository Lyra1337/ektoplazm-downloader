using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using EktoplazmExtractor.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;

namespace EktoplazmExtractor.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public string WindowTitle { get; } = "Ektoplazm.com Downloader";

        private string url = "http://www.ektoplazm.com/style/darkpsy";
        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<Transfer> Transfers { get; } = new ObservableCollection<Transfer>();

        public RelayCommand StartDownloadCommand { get; }

        public MainWindowViewModel()
        {
            this.StartDownloadCommand = new RelayCommand(this.StartDownload, () => Uri.IsWellFormedUriString(this.Url, UriKind.Absolute) == true);
            this.PropertyChanged += this.MainWindowViewModel_PropertyChanged;
        }

        private void MainWindowViewModel_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.Url))
            {
                this.StartDownloadCommand.RaiseCanExecuteChanged();
            }
        }

        private async void StartDownload()
        {
            var http = ServiceLocator.Current.GetInstance<HttpTransmissionService>();

            var ekto = ServiceLocator.Current.GetInstance<EktoplazmParserService>();

            var asd = await ekto.ParseAlbums(this.Url);

            
        }
    }
}