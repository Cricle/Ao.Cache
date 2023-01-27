using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public static class DataFinderOptionsExtensions
    {
        public static IDataFinderOptions<TIdentity, TEntity> WithRenew<TIdentity, TEntity>(this IDataFinderOptions<TIdentity, TEntity> options, bool rewnew)
        {
            if (options is DefaultDataFinderOptions<TIdentity, TEntity> opt)
            {
                opt.IsCanRenewal = rewnew;
                return options;
            }
            throw new InvalidCastException($"Can't cast {options.GetType()} to {typeof(DefaultDataFinderOptions<TIdentity, TEntity>)}");
        }
        public static IDataFinderOptions<TIdentity, TEntity> WithCacheTime<TIdentity, TEntity>(this IDataFinderOptions<TIdentity, TEntity> options, TimeSpan? cacheTime)
        {
            if (options is DefaultDataFinderOptions<TIdentity, TEntity> opt)
            {
                opt.CacheTime = cacheTime;
                return options;
            }
            throw new InvalidCastException($"Can't cast {options.GetType()} to {typeof(DefaultDataFinderOptions<TIdentity, TEntity>)}");
        }
    }
    public interface IWithDataFinderOptions<TIdentity, TEntity>
    {
        IDataFinderOptions<TIdentity, TEntity> Options { get; set; }
    }
    public interface IBatchDataFinder : IBatchCacheFinder, IBatchPhysicalFinder, IBatchRenewalable
    {
        Task<long> DeleteAsync(IList identity);

        Task<IDictionary<object, bool>> ExistsAsync(IList identity);
    }
    public interface IBatchDataFinder<TIdentity, TEntity> : IWithDataFinderOptions<TIdentity, TEntity>, IBatchCacheFinder<TIdentity, TEntity>, IBatchPhysicalFinder<TIdentity, TEntity>, IBatchRenewalable<TIdentity>
    {
        Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity);

        Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity);
    }
}
