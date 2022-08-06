namespace Ao.Cache
{
    public static class CreateEmptyFinderExtensions
    {
        public static IDataFinder<TIdentity, TEntity> CreateEmpty<TIdentity, TEntity>(this IDataFinderFactory<TIdentity, TEntity> factory)
        {
            return factory.Create(new EmptyDataFinderFactory<TIdentity, TEntity>());
        }
        public static IBatchDataFinder<TIdentity, TEntity> CreateEmpty<TIdentity, TEntity>(this IBatchDataFinderFactory<TIdentity, TEntity> factory)
        {
            return factory.Create(new EmptyBatchDataFinderFactory<TIdentity, TEntity>());
        }
    }
}
