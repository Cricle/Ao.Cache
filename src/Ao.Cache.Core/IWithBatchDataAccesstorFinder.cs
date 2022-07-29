namespace Ao.Cache
{
    public interface IWithBatchDataAccesstorFinder<TIdentity, TEntry>
    {
        IBatchDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }
    }
}
