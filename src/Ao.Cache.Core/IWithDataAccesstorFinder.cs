namespace Ao.Cache
{
    public interface IWithDataAccesstorFinder<TIdentity, TEntry>
    {
        IDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }
    }
}
