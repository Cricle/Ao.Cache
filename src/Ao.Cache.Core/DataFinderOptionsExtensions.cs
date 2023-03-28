using System;

namespace Ao.Cache
{
    public static class DataFinderOptionsExtensions
    {
        public static IDataFinderOptions<TIdentity, TEntity> WithHead<TIdentity, TEntity>(this IDataFinderOptions<TIdentity, TEntity> options, string head)
        {
            if (options is DefaultDataFinderOptions<TIdentity, TEntity> opt)
            {
                opt.Head = head;
                return options;
            }

            throw new InvalidCastException(ThrowMessage(options));
        }
        public static IDataFinderOptions<TIdentity, TEntity> WithRenew<TIdentity, TEntity>(this IDataFinderOptions<TIdentity, TEntity> options, bool rewnew)
        {
            if (options is DefaultDataFinderOptions<TIdentity, TEntity> opt)
            {
                opt.IsCanRenewal = rewnew;
                return options;
            }
            throw new InvalidCastException(ThrowMessage(options));
        }
        public static IDataFinderOptions<TIdentity, TEntity> WithCacheTime<TIdentity, TEntity>(this IDataFinderOptions<TIdentity, TEntity> options, TimeSpan? cacheTime)
        {
            if (options is DefaultDataFinderOptions<TIdentity, TEntity> opt)
            {
                opt.CacheTime = cacheTime;
                return options;
            }
            throw new InvalidCastException(ThrowMessage(options));
        }
        private static string ThrowMessage<TIdentity, TEntity>(IDataFinderOptions<TIdentity, TEntity> options)
        {
            return $"Can't cast {options.GetType()} to {typeof(DefaultDataFinderOptions<TIdentity, TEntity>)}";
        }
    }
}
