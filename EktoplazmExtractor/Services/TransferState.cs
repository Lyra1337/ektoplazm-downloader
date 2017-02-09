namespace EktoplazmExtractor.Services
{
    internal enum TransferState : byte
    {
        None,
        Waiting,
        Started,
        Completed,
        Error
    }
}