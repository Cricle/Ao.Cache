namespace Ao.Cache.Serizlier.TextJson
{
    public class TextJsonObjectTransfer : IObjectTransfer
    {
        public static readonly TextJsonObjectTransfer Instance = new TextJsonObjectTransfer();

        private TextJsonObjectTransfer() { }

        public byte[] Transfer<T>(T obj)
        {
            return TextJsonEntityConvertor<T>.Default.ToBytes(obj);
        }

        public T Transfer<T>(byte[] data)
        {
            return TextJsonEntityConvertor<T>.Default.ToEntry(data);
        }

        public T TransferByString<T>(string data)
        {
            return TextJsonEntityConvertor<T>.Default.ToEntry(data);
        }

        public string TransferToString<T>(T obj)
        {
            return TextJsonEntityConvertor<T>.Default.ToString(obj);
        }
    }

}
