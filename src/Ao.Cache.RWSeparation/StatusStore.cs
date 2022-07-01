namespace Ao.Cache.RWSeparation
{
    public abstract class StatusStoreFinder<TStatus,TIdentity,TEntity>
    {
        public IStatusDistribution<TStatus> StatusDistribution { get; }

        public abstract IMixedDataFinder<TIdentity, TEntity> GetDataFinder(TStatus status, TIdentity identity);
    }
    public class Read
    {

    }
    public class Write
    {

    }
}
