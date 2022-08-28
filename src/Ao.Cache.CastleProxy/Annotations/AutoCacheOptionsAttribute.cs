using Ao.Cache.CastleProxy.Exceptions;
using Ao.Cache.CastleProxy.Interceptors;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AutoCacheOptionsAttribute : AutoCacheDecoratorBaseAttribute
    {
        public static readonly object LockerKey = "Features.Locker";

        public static TimeSpan? DefaultCacheTime { get; set; } = TimeSpan.FromSeconds(3);

        public static TimeSpan DefaultLockTime { get; set; } = TimeSpan.FromSeconds(20);

        public static bool DefaultCanRenewal { get; set; } = false;

        public static bool DefaultRenewal { get; set; } = false;

        public static bool DefaultLock { get; set; } = false;

        public static bool DefaultMethodCallDirectCache { get; set; } = true;

        public AutoCacheOptionsAttribute()
        {
        }

        public AutoCacheOptionsAttribute(string cacheTimeStr = null, string lockTime = null)
        {
            if (!string.IsNullOrEmpty(cacheTimeStr))
            {
                CacheTime = TimeSpan.Parse(cacheTimeStr);
            }
            if (!string.IsNullOrEmpty(lockTime))
            {
                LockTime = TimeSpan.Parse(lockTime);
            }
        }

        /// <summary>
        /// Support <see cref="IDataFinderOptions{TIdentity, TEntity}.CanRenewal(TIdentity)"/> result
        /// </summary>
        public bool CanRenewal { get; set; } = DefaultCanRenewal;

        /// <summary>
        /// When <see cref="Renewal"/> is <see langword="true"/> support cache expire time
        /// </summary>
        public TimeSpan? CacheTime { get; } = DefaultCacheTime;

        /// <summary>
        /// Whether cache hit, renew cahce time
        /// </summary>
        public bool Renewal { get; set; } = DefaultRenewal;

        /// <summary>
        /// Whether method call use distributed lock.
        /// </summary>
        public bool Lock { get; set; } = DefaultLock;

        /// <summary>
        /// When <see cref="Lock"/> is <see langword="true"/> it will support to lock expire time
        /// </summary>
        public TimeSpan LockTime { get; set; } = DefaultLockTime;

        /// <summary>
        /// When cache miss, method call, result direct write in cache when result is not null
        /// </summary>
        public bool MethodCallDirectCache { get; set; } = DefaultMethodCallDirectCache;

        public override async Task FindInMethodBeginAsync<TResult>(AutoCacheDecoratorContext<TResult> context, AutoCacheResultBox<TResult> resultBox)
        {
            if (Lock)
            {
                var lockFactory = context.ServiceProvider.GetRequiredService<ILockerFactory>();
                var nameHelper = context.ServiceProvider.GetRequiredService<ICacheNamedHelper>();
                var lockResult = await LockHelper.GetLockAsync(context.Invocation, lockFactory, nameHelper, LockTime);
                if (lockResult.Type != RunLockResultTypes.SkipNoLocker)
                {
                    if (!lockResult.IsLocked)
                    {
                        await GetLockFailAsync(context, lockResult);
                    }
                    var res = await context.DataFinder.FindInCacheAsync(context.Identity);
                    if (res != null)
                    {
                        resultBox.SetResult(res);
                        lockResult.Dispose();
                    }
                    else
                    {
                        context.Features[LockerKey] = lockResult.Locker;
                    }
                }
            }
        }
        public override async Task FindInMethodFinallyAsync<TResult>(AutoCacheDecoratorContext<TResult> context)
        {
            if (Lock)
            {
                if (context.Features.Contains(LockerKey) && context.Features[LockerKey] is ILocker locker)
                {
                    locker.Dispose();
                }
            }
            if (MethodCallDirectCache)
            {
                if (context.Result.RawData != null)
                {
                    await context.DataFinder.SetInCacheAsync(context.Identity, context.Result.RawData);
                }
            }
        }
        protected virtual Task GetLockFailAsync<TResult>(AutoCacheDecoratorContext<TResult> context, RunLockResult lockResult)
        {
            throw new GetLockFailException { locker = lockResult.Locker };
        }
        public override Task DecorateAsync<TResult>(AutoCacheDecoratorContext<TResult> context)
        {
            DefaultDataFinderOptions<UnwindObject, TResult> opt;
            if (context.DataFinder.Options is DefaultDataFinderOptions<UnwindObject, TResult> options)
            {
                opt = options;
            }
            else
            {
                opt = IgnoreHeadDataFinderOptions<TResult>.Options;
                context.DataFinder.Options = opt;
            }
            opt.CacheTime = CacheTime;
            opt.IsCanRenewal = CanRenewal;
            return TaskHelper.ComplatedTask;
        }
        public override async Task FoundInCacheAsync<TResult>(AutoCacheDecoratorContext<TResult> context, TResult result)
        {
            if (Renewal)
            {
                await context.DataFinder.RenewalAsync(context.Identity);
            }
        }
    }
}
