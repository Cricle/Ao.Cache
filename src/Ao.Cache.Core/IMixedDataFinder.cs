namespace Ao.Cache
{
    public interface IMixedDataFinder<TIdentity, TEntity> : IDataFinder<TIdentity, TEntity>, IBatchDataFinder<TIdentity, TEntity>
    {

    }
}
