namespace Ao.Cache.CastleProxy
{
    public interface IStringTransfer
    {
        string ToString(object data);

        string Combine(params object[] args);
    }
}
