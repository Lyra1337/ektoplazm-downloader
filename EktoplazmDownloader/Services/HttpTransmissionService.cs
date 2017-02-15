using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Practices.ServiceLocation;

namespace EktoplazmExtractor.Services
{
    internal class HttpTransmissionService
    {
        private readonly Queue<Transfer> transferQueue = new Queue<Transfer>();
        private Thread workerThread = null;
        private bool isRunning = false;

        public Transfer Enqueue(string remoteUrl, string localPath, Album album)
        {
            var transfer = new Transfer(remoteUrl, localPath, album);

            this.transferQueue.Enqueue(transfer);

            return transfer;
        }

        public void Start()
        {
            if (this.isRunning == true)
            {
                throw new Exception("already running");
            }

            this.isRunning = true;

            this.workerThread = new Thread(this.TransferWorker);
            this.workerThread.Start();
        }

        internal void Stop()
        {
            this.isRunning = false;
        }

        private void TransferWorker()
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(4, 4);

            while (this.isRunning == true)
            {
                if (this.transferQueue.Any() == true && semaphore.CurrentCount > 0)
                {
                    try
                    {
                        var currentTransfer = this.transferQueue.Dequeue();

                        semaphore.Wait();

                        WebClient client = new WebClient();
                        
                        client.DownloadProgressChanged += this.Client_DownloadProgressChanged;
                        client.DownloadFileCompleted += (s, e) =>
                        {
                            semaphore.Release();
                            this.Client_DownloadFileCompleted(s, e);
                        };

                        client.DownloadFileAsync(new Uri(currentTransfer.RemoteUrl), currentTransfer.LocalPath, currentTransfer);

                        currentTransfer.State = TransferState.Started;
                    }
                    catch (Exception)
                    {
                        // TODO
                        //this.transferQueue.Enqueue(currentTransfer);
                    }
                }

                Thread.Sleep(100);
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var transfer = (Transfer)e.UserState;

            transfer.State = TransferState.DownloadCompleted;
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var transfer = (Transfer)e.UserState;

            transfer.Progress = e.TotalBytesToReceive / Convert.ToSingle(e.BytesReceived);
            transfer.State = TransferState.Tranferring;
        }
    }
}