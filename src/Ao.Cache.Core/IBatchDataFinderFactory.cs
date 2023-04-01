namespace Ao.Cache
{
    public interface IBatchDataFinderFactory
    {
        IBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>();

        IWithBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>(IBatchDataAccesstor<TIdentity, TEntity> accesstor);
    }
}
