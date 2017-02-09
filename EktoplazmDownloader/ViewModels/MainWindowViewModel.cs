using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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

        public RelayCommand StartDownloadCommand { get; }

        public MainWindowViewModel()
        {
            this.StartDownloadCommand = new RelayCommand(this.StartDownload, () => Uri.IsWellFormedUriString(this.Url, UriKind.Absolute) == true);
        }

        private void StartDownload()
        {

        }
    }
}