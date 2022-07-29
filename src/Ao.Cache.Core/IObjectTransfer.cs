namespace Ao.Cache
{
    public interface IObjectTransfer
    {
        byte[] Transfer<T>(T obj);

        string TransferToString<T>(T obj);

        T Transfer<T>(byte[] data);

        T TransferByString<T>(string data);
    }
}
