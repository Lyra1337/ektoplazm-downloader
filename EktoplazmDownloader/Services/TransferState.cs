namespace EktoplazmExtractor.Services
{
    internal enum TransferState : byte
    {
        None,
        Waiting,
        Started,
        Tranferring,
        DownloadCompleted,
        Error,
        Finished
    }
}