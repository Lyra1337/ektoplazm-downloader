using System;
using GalaSoft.MvvmLight;

namespace EktoplazmExtractor.Services
{
    internal sealed class Transfer : ViewModelBase
    {
        public string RemoteUrl { get; }

        public string LocalPath { get; }

        private TransferState state;
        public TransferState State
        {
            get
            {
                return this.state;
            }
            set
            {
                if (this.state != value)
                {
                    this.state = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private float progress;
        public float Progress
        {
            get
            {
                return this.progress;
            }
            set
            {
                this.progress = value;
                this.RaisePropertyChanged();
            }
        }

        public Album Album { get; }

        public Transfer(string remoteUrl, string localPath, Album album)
        {
            this.RemoteUrl = remoteUrl;
            this.LocalPath = localPath;
            this.Album = album;
        }
    }
}