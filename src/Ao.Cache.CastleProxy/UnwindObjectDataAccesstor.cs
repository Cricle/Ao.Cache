using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy
{
    public class CastleDataAccesstor<TKey, TEntity> : IDataAccesstor<TKey, TEntity>
    {
        public Func<Task<TEntity>> Proceed { get; set; }

        public Task<TEntity> FindAsync(TKey identity)
        {
            return Proceed();
        }
    }
}
