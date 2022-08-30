namespace Ao.Cache.CastleProxy
{
    public interface IStringTransfer
    {
        string ToString(object data);

        string Combine(object header, params object[] args);
    }
}
