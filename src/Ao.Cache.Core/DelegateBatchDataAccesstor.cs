using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public readonly struct DelegateSyncBatchDataAccesstor<TIdentity, TEntity> : ISyncBatchDataAccesstor<TIdentity, TEntity>
    {
        private readonly Func<IReadOnlyList<TIdentity>, IDictionary<TIdentity, TEntity>> func;

        public DelegateSyncBatchDataAccesstor(Func<IReadOnlyList<TIdentity>, IDictionary<TIdentity, TEntity>> func)
        {
            this.func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public IDictionary<TIdentity, TEntity> Find(IReadOnlyList<TIdentity> identity)
        {
            return func(identity);
        }
    }
    public readonly struct DelegateBatchDataAccesstor<TIdentity, TEntity> : IBatchDataAccesstor<TIdentity, TEntity>
    {
        private readonly Func<IReadOnlyList<TIdentity>, Task<IDictionary<TIdentity, TEntity>>> func;

        public DelegateBatchDataAccesstor(Func<IReadOnlyList<TIdentity>, Task<IDictionary<TIdentity, TEntity>>> func)
        {
            this.func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public Task<IDictionary<TIdentity, TEntity>> FindAsync(IReadOnlyList<TIdentity> identity)
        {
            return func(identity);
        }
    }
}
