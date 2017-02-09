using System;
using System.Collections.Generic;
using System.Net;

namespace EktoplazmExtractor.Services
{
    internal class HttpTransmissionService
    {
        private readonly Queue<Transfer> transferQueue = new Queue<Transfer>();

        public HttpTransmissionService()
        {

        }

        public Transfer Enqueue(string remoteUrl, string localPath)
        {
            var transfer = new Transfer(remoteUrl, localPath);

            this.transferQueue.Enqueue(transfer);

            return transfer;
        }

        private void TransferWorker()
        {
            while (true)
            {
                var currentTransfer = this.transferQueue.Dequeue();

                try
                {
                    WebClient client = new WebClient();

                    client.DownloadFileAsync(new Uri(currentTransfer.RemoteUrl), currentTransfer.LocalPath + "test.zip", currentTransfer);

                    //client.DownloadProgressChanged += (sender, e) => currentTransfer.Progress = (float)(e.TotalBytesToReceive / (double)e.BytesReceived);
                    client.DownloadProgressChanged += this.Client_DownloadProgressChanged;
                }
                catch (Exception)
                {
                    //this.transferQueue.Enqueue(currentTransfer);
                }
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var transfer = (Transfer)e.UserState;

            transfer.Progress = (float)(e.TotalBytesToReceive / (double)e.BytesReceived);
            transfer.State = e.ProgressPercentage == 100 ? TransferState.Completed : TransferState.Started;
        }
    }
}