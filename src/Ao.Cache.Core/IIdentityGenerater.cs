namespace Ao.Cache
{
    public interface IIdentityGenerater<TIdentity>
    {
        bool IgnoreHead { get; set; }

        string GetPart(TIdentity identity);
        string GetHead();
        string GetEntryKey(TIdentity identity);
    }

}
