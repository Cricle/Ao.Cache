namespace Ao.Cache
{
    public interface IBatchDataFinderFactory
    {
        IBatchDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IBatchDataAccesstor<TIdentity, TEntity> accesstor);
    }
}
