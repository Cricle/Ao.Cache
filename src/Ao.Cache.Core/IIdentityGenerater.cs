namespace Ao.Cache
{
    public interface IWithHeaderIdentityGenerater<TIdentity> : IIdentityGenerater<TIdentity>
    {
        bool IgnoreHead { get; set; }

    }
    public interface IIdentityGenerater<TIdentity>
    {
        string GetPart(TIdentity identity);
        string GetHead();
        string GetEntryKey(TIdentity identity);
    }

}
