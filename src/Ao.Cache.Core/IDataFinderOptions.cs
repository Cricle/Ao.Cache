using System;

namespace Ao.Cache
{
    public interface IDataFinderOptions<TIdentity, TEntity> : IIdentityGenerater<TIdentity>
    {
        TimeSpan? GetCacheTime(TIdentity identity);

        bool CanRenewal(TIdentity identity);
    }

}
