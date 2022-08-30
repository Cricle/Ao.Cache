using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public class BatchDataFinderGroup<TIdentity, TEntity> : List<IBatchDataFinder<TIdentity, TEntity>>, IBatchDataFinder<TIdentity, TEntity>
    {
        public BatchDataFinderGroup()
        {
        }

        public BatchDataFinderGroup(IEnumerable<IBatchDataFinder<TIdentity, TEntity>> collection) : base(collection)
        {
        }

        public BatchDataFinderGroup(int capacity) : base(capacity)
        {
        }

        public IDataFinderOptions<TIdentity, TEntity> Options
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }
                return this[0].Options;
            }
            set
            {
                value = value ?? DefaultDataFinderOptions<TIdentity, TEntity>.Default;
                foreach (var item in this)
                {
                    item.Options = value;
                }
            }
        }

        public async Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity)
        {
            if (Count == 0)
            {
                return 0;
            }
            var tasks = new Task<long>[Count];
            for (int i = 0; i < Count; i++)
            {
                tasks[i] = this[i].DeleteAsync(identity);
            }
            await Task.WhenAll(tasks);
            return tasks.Max(x => x.Result);
        }

        public async Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity)
        {
            var exists = identity.ToList();
            var res = new Dictionary<TIdentity, bool>(identity.Count);
            for (int i = 0; i < Count; i++)
            {
                var entity = this[i];
                var data = await entity.ExistsAsync(exists);
                foreach (var item in data)
                {
                    res[item.Key] = true;
                }
                exists.RemoveAll(x => exists.Contains(x));
                if (exists.Count == 0)
                {
                    break;
                }
            }
            if (exists.Count != 0)
            {
                for (int i = 0; i < exists.Count; i++)
                {
                    var item = exists[i];
                    if (!res.ContainsKey(item))
                    {
                        res[item] = false;
                    }
                }
            }
            return res;
        }

        public async Task<IDictionary<TIdentity, TEntity>> FindInCacheAsync(IReadOnlyList<TIdentity> identity)
        {
            var exists = identity.ToList();
            var res = new Dictionary<TIdentity, TEntity>(identity.Count);
            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                var r = await item.FindInCacheAsync(exists);
                foreach (var it in r)
                {
                    res[it.Key] = it.Value;
                }
                exists.RemoveAll(x => r.ContainsKey(x));
            }
            return res;
        }

        public async Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IReadOnlyList<TIdentity> identity, bool cache)
        {
            var exists = identity.ToList();
            var res = new Dictionary<TIdentity, TEntity>(identity.Count);
            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                var r = await item.FindInDbAsync(exists, false);
                foreach (var it in r)
                {
                    res[it.Key] = it.Value;
                }
                exists.RemoveAll(x => r.ContainsKey(x));
            }
            if (cache && res.Count != 0)
            {
                await SetInCacheAsync(res);
            }
            return res;
        }
        protected virtual bool IsHit(TEntity entity)
        {
            return entity != null;
        }
        public async Task<long> RenewalAsync(IReadOnlyList<TIdentity> input)
        {
            if (Count == 0)
            {
                return 0;
            }
            var tasks = new Task<long>[Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = this[i].RenewalAsync(input);
            }
            await Task.WhenAll(tasks);
            return tasks.Max(x => x.Result);
        }

        public async Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input)
        {
            if (Count == 0)
            {
                return 0;
            }
            var tasks = new Task<long>[Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = this[i].RenewalAsync(input);
            }
            await Task.WhenAll(tasks);
            return tasks.Max(x => x.Result);
        }

        public async Task<long> SetInCacheAsync(IDictionary<TIdentity, TEntity> pairs)
        {
            if (Count == 0)
            {
                return 0;
            }
            var tasks = new Task<long>[Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = this[i].SetInCacheAsync(pairs);
            }
            await Task.WhenAll(tasks);
            return tasks.Max(x => x.Result);
        }
    }

}
