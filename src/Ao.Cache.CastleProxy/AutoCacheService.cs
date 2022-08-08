using Ao.Cache.CastleProxy.Interceptors;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy
{
    public class AutoCacheService<TEntity>
    {
        public AutoCacheService(IDataFinderFactory finderFactory, ICacheNamedHelper namedHelper)
        {
            FinderFactory = finderFactory ?? throw new System.ArgumentNullException(nameof(finderFactory));
            NamedHelper = namedHelper ?? throw new System.ArgumentNullException(nameof(namedHelper));
        }

        public IDataFinderFactory FinderFactory { get; }

        public ICacheNamedHelper NamedHelper { get; }

        public IDataFinder<UnwindObject, TEntity> GetEmpty()
        {
            var finder = FinderFactory.CreateEmpty<UnwindObject,TEntity>();
            SetIgnoreHead(finder);
            return finder;
        }
        public IDataFinder<UnwindObject, TEntity> Get(IDataAccesstor<UnwindObject, TEntity> accesstor)
        {
            var finder = FinderFactory.Create(accesstor);
            SetIgnoreHead(finder);
            return finder;
        }
        private static void SetIgnoreHead(IDataFinder<UnwindObject, TEntity> finder)
        {
            if (finder is DataFinderBase<UnwindObject, TEntity> igen)
            {
                igen.Options = IgnoreHeadDataFinderOptions<TEntity>.Options;
            }
        }
        public Task<bool> DeleteAsync(in UnwindObject unwindObject)
        {
            var finder = GetEmpty();
            return finder.DeleteAsync(unwindObject);
        }
    }
}
