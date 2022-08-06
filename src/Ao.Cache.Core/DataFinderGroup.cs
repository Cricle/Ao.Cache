using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public class DataFinderGroup<TIdentity, TEntity> : List<IDataFinder<TIdentity, TEntity>>, IDataFinder<TIdentity, TEntity>
    {
        public IDataFinderOptions<TIdentity, TEntity> Options
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public virtual async Task<bool> DeleteAsync(TIdentity identity)
        {
            var tasks = new Task[Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = this[i].DeleteAsync(identity);
            }
            await Task.WhenAll(tasks);
            return true;
        }

        public virtual async Task<bool> ExistsAsync(TIdentity identity)
        {
            for (int i = 0; i < Count; i++)
            {
                var entity = this[i];
                var data = await entity.ExistsAsync(identity);
                if (data)
                {
                    return data;
                }
            }
            return false;
        }

        public virtual async Task<TEntity> FindInCahceAsync(TIdentity identity)
        {
            for (int i = 0; i < Count; i++)
            {
                var entity = this[i];
                var data = await entity.FindInCahceAsync(identity);
                if (IsHit(data))
                {
                    return data;
                }
            }
            return default;
        }

        public virtual async Task<TEntity> FindInDbAsync(TIdentity identity, bool cache)
        {
            for (int i = 0; i < Count; i++)
            {
                var entity = this[i];
                var data = await entity.FindInDbAsync(identity, false);
                if (IsHit(data))
                {
                    if (cache)
                    {
                        await SetInCahceAsync(identity, data);
                    }
                    return data;
                }
            }
            return default;
        }

        public async Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            var tasks = new Task[Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = this[i].RenewalAsync(identity, time);
            }
            await Task.WhenAll(tasks);
            return true;
        }

        public virtual async Task<bool> SetInCahceAsync(TIdentity identity, TEntity entity)
        {
            var tasks = new Task[Count];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = this[i].SetInCahceAsync(identity, entity);
            }
            await Task.WhenAll(tasks);
            return true;
        }

        protected virtual bool IsHit(TEntity entity)
        {
            return entity != null;
        }
    }

}
