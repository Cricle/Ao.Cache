using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public class DelegateBatchDataAccesstor<TIdentity, TEntity> : IBatchDataAccesstor<TIdentity, TEntity>
    {
        private readonly Func<IReadOnlyList<TIdentity>, Task<IDictionary<TIdentity, TEntity>>> func;

        public DelegateBatchDataAccesstor(Func<IReadOnlyList<TIdentity>, Task<IDictionary<TIdentity, TEntity>>> func)
        {
            this.func = func;
        }

        public Task<IDictionary<TIdentity, TEntity>> FindAsync(IReadOnlyList<TIdentity> identity)
        {
            return func(identity);
        }
    }
}
