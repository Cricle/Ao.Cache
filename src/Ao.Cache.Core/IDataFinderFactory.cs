namespace Ao.Cache
{
    public interface IDataFinderFactory
    {
        IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IDataAccesstor<TIdentity, TEntity> accesstor);
    }
}
