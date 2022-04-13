using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class DataFinderGroup<TIdentity, TEntity> :List<IDataFinder<TIdentity, TEntity>>, IDataFinder<TIdentity, TEntity>
    {
        public async Task<bool> DeleteAsync(TIdentity identity)
        {
            var count = Count;
            var tasks = new Task[count];
            for (int i = 0; i < count; i++)
            {
                tasks[i] = this[i].DeleteAsync(identity);
            }
            await Task.WhenAll(tasks);
            return true;
        }

        public async Task<bool> ExistsAsync(TIdentity identity)
        {
            var count = Count;
            for (int i = 0; i < count; i++)
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

        public async Task<TEntity> FindInCahceAsync(TIdentity identity)
        {
            var count = Count;
            for (int i = 0; i < count; i++)
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

        public abstract Task<TEntity> FindInDbAsync(TIdentity identity, bool cache);

        public virtual async Task<bool> SetInCahceAsync(TIdentity identity, TEntity entity)
        {
            var count = Count;
            var tasks = new Task[count];
            for (int i = 0; i < count; i++)
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
