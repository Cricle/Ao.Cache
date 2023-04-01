using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public static class BatchDataFinderExtensions
    {
        public static Task<TEntity> FindAsync<TIdentity, TEntity>(this IWithDataFinder<TIdentity, TEntity> finder, TIdentity identity, bool cache = true)
        {
            return FindAsync(finder, identity, finder.DataAccesstor, cache);
        }
        public static Task<IDictionary<TIdentity, TEntity>> FindAsync<TIdentity, TEntity>(this IWithBatchDataFinder<TIdentity, TEntity> finder, IReadOnlyList<TIdentity> identities, bool cache = true)
        {
            return FindAsync(finder, identities, finder.DataAccesstor, cache);
        }
        public static async Task<TEntity> FindAsync<TIdentity, TEntity>(this IDataFinder<TIdentity, TEntity> finder, TIdentity identity,IDataAccesstor<TIdentity,TEntity> dataAccesstor, bool cache = true)
        {
            if (finder is null)
            {
                throw new ArgumentNullException(nameof(finder));
            }
            var val = await finder.FindInCacheAsync(identity).ConfigureAwait(false);
            if (val == null)
            {
                return await finder.FindInDbAsync(dataAccesstor,identity, cache).ConfigureAwait(false);
            }
            return val;
        }
        public static async Task<IDictionary<TIdentity, TEntity>> FindAsync<TIdentity, TEntity>(this IBatchDataFinder<TIdentity, TEntity> finder, IReadOnlyList<TIdentity> identities, IBatchDataAccesstor<TIdentity, TEntity> batchDataAccesstor, bool cache = true)
        {
            if (finder is null)
            {
                throw new ArgumentNullException(nameof(finder));
            }

            if (identities is null)
            {
                throw new ArgumentNullException(nameof(identities));
            }

            var cacheDatas = await finder.FindInCacheAsync(identities).ConfigureAwait(false);
            var notIncludes = identities.Except(cacheDatas.Keys);
            if (!notIncludes.Any())
            {
                return cacheDatas;
            }

            var dbDatas = await finder.FindInDbAsync(batchDataAccesstor, notIncludes.ToList(), cache).ConfigureAwait(false);

            foreach (var item in dbDatas)
            {
                cacheDatas[item.Key] = item.Value;
            }

            return cacheDatas;
        }
    }
}
