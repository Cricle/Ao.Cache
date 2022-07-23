namespace Ao.Cache.CastleProxy
{
    public interface IUnwindObject
    {
        object[] Objects { get; }

        string ToString();
    }
}
