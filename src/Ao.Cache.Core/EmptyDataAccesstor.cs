using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public class EmptyDataAccesstor<TIdentity, TEntity> : IDataAccesstor<TIdentity, TEntity>, IBatchDataAccesstor<TIdentity, TEntity>
    {
        private EmptyDataAccesstor() { }

        public static readonly EmptyDataAccesstor<TIdentity, TEntity> Instance = new EmptyDataAccesstor<TIdentity, TEntity>();

        public Task<TEntity> FindAsync(TIdentity identity)
        {
            throw new NotImplementedException();
        }

        public Task<IDictionary<TIdentity, TEntity>> FindAsync(IReadOnlyList<TIdentity> identities)
        {
            throw new NotImplementedException();
        }
    }
}
