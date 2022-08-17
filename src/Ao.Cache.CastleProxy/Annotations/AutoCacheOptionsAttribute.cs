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

        public static readonly TimeSpan? DefaultCacheTime = TimeSpan.FromSeconds(3);

        public static readonly TimeSpan DefaultLockTime = TimeSpan.FromSeconds(20);

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

        public bool CanRenewal { get; set; }

        public TimeSpan? CacheTime { get; } = DefaultCacheTime;

        public bool Renewal { get; set; }

        public bool Lock { get; set; }

        public TimeSpan LockTime { get; set; } = DefaultLockTime;

        public override async Task InterceptBeginAsync<TResult>(AutoCacheInvokeDecoratorContext<TResult> context)
        {
            if (Lock)
            {
                using (var scope = context.ServiceScopeFactory.CreateScope())
                {
                    var lockFactory = scope.ServiceProvider.GetRequiredService<ILockerFactory>();
                    var nameHelper = scope.ServiceProvider.GetRequiredService<ICacheNamedHelper>();
                    var lockResult = await LockHelper.GetLockAsync(context.Invocation, lockFactory, nameHelper, LockTime);
                    if (lockResult.Type != RunLockResultTypes.SkipNoLocker)
                    {
                        if (!lockResult.IsLocked)
                        {
                            await GetLockFailAsync(context, lockResult);
                        }
                        context.Feature[LockerKey] = lockResult.Locker;
                    }
                }
            }
        }
        protected virtual Task GetLockFailAsync<TResult>(AutoCacheInvokeDecoratorContext<TResult> context, RunLockResult lockResult)
        {
            throw new GetLockFailException { locker = lockResult.Locker };
        }
        public override Task InterceptFinallyAsync<TResult>(AutoCacheInvokeDecoratorContext<TResult> context)
        {
            if (Lock)
            {
                try
                {
                    if (context.Feature.Contains(LockerKey) && context.Feature[LockerKey] is ILocker locker)
                    {
                        locker.Dispose();
                    }
                }
                catch { }
            }
            return base.InterceptFinallyAsync(context);
        }
        public override Task DecorateAsync<TResult>(AutoCacheDecoratorContext<TResult> context)
        {
            DefaultDataFinderOptions<UnwindObject, TResult> opt = null;
            if (context.DataFinder.Options is DefaultDataFinderOptions<UnwindObject, TResult>)
            {
                opt = (DefaultDataFinderOptions<UnwindObject, TResult>)context.DataFinder.Options;
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
