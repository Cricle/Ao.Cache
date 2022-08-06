namespace Ao.Cache
{
    public interface IDataFinderFactory<TIdentity, TEntity>
    {
        IDataFinder<TIdentity, TEntity> Create(IDataAccesstor<TIdentity, TEntity> accesstor);
    }
}
