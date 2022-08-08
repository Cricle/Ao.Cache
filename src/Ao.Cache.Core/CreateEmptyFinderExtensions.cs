namespace Ao.Cache
{
    public static class CreateEmptyFinderExtensions
    {
        public static IDataFinder<TIdentity, TEntity> CreateEmpty<TIdentity, TEntity>(this IDataFinderFactory factory)
        {
            return factory.Create(EmptyDataAccesstor<TIdentity, TEntity>.Instance);
        }
        public static IBatchDataFinder<TIdentity, TEntity> CreateEmpty<TIdentity, TEntity>(this IBatchDataFinderFactory factory)
        {
            return factory.Create(EmptyDataAccesstor<TIdentity, TEntity>.Instance);
        }
    }
}
