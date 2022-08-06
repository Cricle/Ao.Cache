namespace Ao.Cache
{
    public static class CreateEmptyFinderExtensions
    {
        public static IDataFinder<TIdentity, TEntity> CreateEmpty<TIdentity, TEntity>(this IDataFinderFactory<TIdentity, TEntity> factory)
        {
            return factory.Create(EmptyDataFinderFactory<TIdentity, TEntity>.Instance);
        }
        public static IBatchDataFinder<TIdentity, TEntity> CreateEmpty<TIdentity, TEntity>(this IBatchDataFinderFactory<TIdentity, TEntity> factory)
        {
            return factory.Create(EmptyBatchDataFinderFactory<TIdentity, TEntity>.Instance);
        }
    }
}
