using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Ao.Cache.InMemory
{
    public class DefaultInMemoryCacheFinder<TIdentity, TEntry> : InMemoryCacheFinder<TIdentity, TEntry>,IWithDataFinder<TIdentity, TEntry>
    {
        public DefaultInMemoryCacheFinder(IMemoryCache memoryCache, IDataAccesstor<TIdentity, TEntry> dataAccesstor)
            :base(memoryCache)
        {
            DataAccesstor = dataAccesstor;
        }

        public IDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        public async Task<TEntry> FindInDbAsync(TIdentity identity, bool cache)
        {
            var entity = await DataAccesstor.FindAsync(identity);
            if (cache&&entity!=null)
            {
                await SetInCacheAsync(identity, entity);
            }
            return entity;
        }
    }
}
