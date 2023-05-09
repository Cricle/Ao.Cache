namespace Ao.Cache
{
    public interface IBatchDataFinderFactory
    {
        IBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>();

        IWithBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>(IBatchDataAccesstor<TIdentity, TEntity> accesstor);
    }
    public interface ISyncBatchDataFinderFactory
    {
        ISyncBatchDataFinder<TIdentity, TEntity> CreateBatchSync<TIdentity, TEntity>();

        ISyncWithBatchDataFinder<TIdentity, TEntity> CreateBatchSync<TIdentity, TEntity>(ISyncBatchDataAccesstor<TIdentity, TEntity> accesstor);
    }
}
