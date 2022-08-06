using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy
{
    public struct CastleDataAccesstor<TKey, TEntity> : IDataAccesstor<TKey, TEntity>
    {
        public Func<Task<TEntity>> Proceed;

        public Task<TEntity> FindAsync(TKey identity)
        {
            return Proceed();
        }
    }
}
