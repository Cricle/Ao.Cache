using System;

namespace Ao.Cache
{
    public interface IDataFinderOptions<TIdentity, TEntity> : IIdentityGenerater<TIdentity>
    {
        TimeSpan? GetCacheTime(TIdentity identity, TEntity entity);

        bool CanRenewal(TIdentity identity, TEntity entity);
    }

}
