using System;
using System.Runtime.CompilerServices;

namespace Ao.Cache
{
#if !NETSTANDARD1_0

    public class DataFinders
    {
        protected readonly IServiceProvider provider;

        public DataFinders(IServiceProvider provider)
        {
            {
                this.provider = provider;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDataFinder<TIdentity, TEntity> Get<TIdentity, TEntity>(TimeSpan? cacheTime = null, bool renewal = false)
        {
            var finder = (IDataFinder<TIdentity, TEntity>)provider.GetService(typeof(IDataFinder<TIdentity, TEntity>));
            if (cacheTime != null)
            {
                {
                    finder.Options.WithCacheTime(cacheTime);
                    finder.Options.WithRenew(renewal);
                }
            }
            return finder;
        }
    }
#endif
}
