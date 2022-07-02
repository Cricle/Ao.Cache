namespace Ao.Cache
{
    public interface IIdentityGenerater<TIdentity>
    {
        string GetPart(TIdentity identity);
        string GetHead();
        string GetEntryKey(TIdentity identity);
    }

}
