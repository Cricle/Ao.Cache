using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public static class BatchDataFinderExtensions
    {
        public static async Task<TEntity> FindAsync<TIdentity, TEntity>(this IDataFinder<TIdentity, TEntity> finder, TIdentity identity, bool cache = true)
        {
            if (finder is null)
            {
                throw new ArgumentNullException(nameof(finder));
            }
            var val = await finder.FindInCahceAsync(identity).ConfigureAwait(false);
            if (ReferenceEquals(val, default(TEntity)))
            {
                return await finder.FindInDbAsync(identity, cache).ConfigureAwait(false);
            }
            return val;
        }
        public static async Task<IDictionary<TIdentity, TEntity>> FindAsync<TIdentity, TEntity>(this IBatchDataFinder<TIdentity, TEntity> finder, IReadOnlyList<TIdentity> identities, bool cache = true)
        {
            if (finder is null)
            {
                throw new ArgumentNullException(nameof(finder));
            }

            if (identities is null)
            {
                throw new ArgumentNullException(nameof(identities));
            }

            var cacheDatas = await finder.FindInCahceAsync(identities).ConfigureAwait(false);
            var notIncludes = identities.Except(cacheDatas.Keys);
            if (!notIncludes.Any())
            {
                return cacheDatas;
            }

            var dbDatas = await finder.FindInDbAsync(notIncludes.ToList(), cache).ConfigureAwait(false);

            foreach (var item in dbDatas)
            {
                cacheDatas[item.Key] = item.Value;
            }

            return cacheDatas;
        }
    }
}
