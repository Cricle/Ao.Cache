namespace Ao.Cache
{
    public interface IBatchDataFinderFactory<TIdentity, TEntity>
    {
        IBatchDataFinder<TIdentity, TEntity> Create(IBatchDataAccesstor<TIdentity, TEntity> accesstor);
    }
}
