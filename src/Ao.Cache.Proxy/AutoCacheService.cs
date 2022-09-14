using Ao.Cache.Proxy.Interceptors;

namespace Ao.Cache.Proxy
{
    public class AutoCacheService
    {
        public AutoCacheService(IDataFinderFactory finderFactory, ICacheNamedHelper namedHelper)
        {
            FinderFactory = finderFactory ?? throw new System.ArgumentNullException(nameof(finderFactory));
            NamedHelper = namedHelper ?? throw new System.ArgumentNullException(nameof(namedHelper));
        }

        public IDataFinderFactory FinderFactory { get; }

        public ICacheNamedHelper NamedHelper { get; }

        public IDataFinder<UnwindObject, TEntity> GetEmpty<TEntity>()
        {
            var finder = FinderFactory.CreateEmpty<UnwindObject, TEntity>();
            SetIgnoreHead(finder);
            return finder;
        }
        public IDataFinder<UnwindObject, TEntity> Get<TEntity>(IDataAccesstor<UnwindObject, TEntity> accesstor)
        {
            var finder = FinderFactory.Create(accesstor);
            SetIgnoreHead(finder);
            return finder;
        }
        private static void SetIgnoreHead<TEntity>(IDataFinder<UnwindObject, TEntity> finder)
        {
            if (finder is DataFinderBase<UnwindObject, TEntity> igen)
            {
                igen.Options = IgnoreHeadDataFinderOptions<TEntity>.Options;
            }
        }
    }
}
