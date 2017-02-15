using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using WinForms = System.Windows.Forms;
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

        private string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public string LocalFolder
        {
            get
            {
                return this.localFolder;
            }
            set
            {
                this.localFolder = (value?.Last() ?? Path.DirectorySeparatorChar) == Path.DirectorySeparatorChar ? value : String.Concat(value, Path.DirectorySeparatorChar); //hacky..
                this.RaisePropertyChanged();
            }
        }

        private DownloadType currentDownloadType = null;
        public DownloadType CurrentDownloadType
        {
            get
            {
                return this.currentDownloadType;
            }
            set
            {
                this.currentDownloadType = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<Transfer> Transfers { get; } = new ObservableCollection<Transfer>();
        public ObservableCollection<Album> AvailableDownloads { get; } = new ObservableCollection<Album>();
        public ObservableCollection<DownloadType> DownloadTypes { get; } = new ObservableCollection<DownloadType>();

        public RelayCommand FindDownloadsCommand { get; }
        public RelayCommand ChooseDestinationFolderCommand { get; }
        public RelayCommand StartDownloadCommand { get; }

        private HttpTransmissionService httpTransmissionService = ServiceLocator.Current.GetInstance<HttpTransmissionService>();
        private EktoplazmParserService ektoplazmParserService = ServiceLocator.Current.GetInstance<EktoplazmParserService>();
        private CompressionService compressionService = ServiceLocator.Current.GetInstance<CompressionService>();

        public MainWindowViewModel()
        {
            this.FindDownloadsCommand = new RelayCommand(this.FindDownloadsAction, () => Uri.IsWellFormedUriString(this.Url, UriKind.Absolute) == true);
            this.ChooseDestinationFolderCommand = new RelayCommand(this.ChooseDestinationFolderAction);
            this.StartDownloadCommand = new RelayCommand(this.StartDownloadAction, () => this.AvailableDownloads.Any() == true && this.CurrentDownloadType != null);

            this.AvailableDownloads.CollectionChanged += (s, e) => this.StartDownloadCommand.RaiseCanExecuteChanged();

            this.PropertyChanged += this.MainWindowViewModel_PropertyChanged;
        }

        private void StartDownloadAction()
        {
            this.Transfers.Clear();

            this.AvailableDownloads.ToList().ForEach(x =>
            {
                if (x.Downloads.ContainsKey(this.CurrentDownloadType.Key) == true)
                {
                    var transfer = this.httpTransmissionService.Enqueue(x.Downloads[this.CurrentDownloadType.Key], String.Concat(this.GetFilePath(x), ".zip"), x);
                    transfer.PropertyChanged += this.Transfer_PropertyChanged;
                    this.Transfers.Add(transfer);
                }
            });

            this.httpTransmissionService.Start();
        }

        private void Transfer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var transfer = (Transfer)sender;

            if (e.PropertyName == nameof(Transfer.State) && transfer.State == TransferState.DownloadCompleted)
            {
                (new Thread(() =>
                {
                    this.compressionService.Decompress(transfer.LocalPath, this.GetFilePath(transfer.Album));
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => transfer.State = TransferState.Finished));
                })).Start();
            }
        }

        private String GetFilePath(Album album)
        {
            var name = album.Name;
            Path.GetInvalidFileNameChars().ToList().ForEach(x => name.Replace(x, '_').Replace("__", "_"));
            return String.Concat(this.LocalFolder, name);
        }

        private void ChooseDestinationFolderAction()
        {
            using (var dialog = new WinForms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result == WinForms.DialogResult.OK)
                {
                    this.LocalFolder = dialog.SelectedPath;
                }
            }
        }

        private void MainWindowViewModel_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.Url))
            {
                this.FindDownloadsCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(MainWindowViewModel.CurrentDownloadType))
            {
                this.StartDownloadCommand.RaiseCanExecuteChanged();
            }
        }

        private async void FindDownloadsAction()
        {
            this.AvailableDownloads.Clear();
            this.DownloadTypes.Clear();
            this.CurrentDownloadType = null;

            (await this.ektoplazmParserService.ParseAlbums(this.Url)).ForEach(this.AvailableDownloads.Add);

            this.AvailableDownloads
                .SelectMany(x => x.Downloads)
                .GroupBy(
                    keySelector: x => x.Key,
                    elementSelector: x => x.Key,
                    resultSelector: (x, y) => new DownloadType(x, y.Count())
                )
                .ToList()
                .ForEach(this.DownloadTypes.Add);

            this.CurrentDownloadType = this.DownloadTypes.OrderByDescending(x => x.Count).FirstOrDefault();
        }
    }
}