namespace Ao.Cache.Proxy
{
    public interface IStringTransfer
    {
        string ToString(object data);

        string Combine(object header, params object[] args);
    }
}
