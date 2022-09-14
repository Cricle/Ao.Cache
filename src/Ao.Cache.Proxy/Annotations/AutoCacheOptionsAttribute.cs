using Ao.Cache.Proxy.Exceptions;
using Ao.Cache.Proxy.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Annotations
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

        public bool CanRenewal { get; set; } = DefaultCanRenewal;

        public TimeSpan? CacheTime { get; } = DefaultCacheTime;

        public bool Renewal { get; set; } = DefaultRenewal;

        public bool Lock { get; set; } = DefaultLock;

        public TimeSpan LockTime { get; set; } = DefaultLockTime;

        public override async Task FindInMethodBeginAsync<TResult>(AutoCacheDecoratorContext<TResult> context, AutoCacheResultBox<TResult> resultBox)
        {
            if (Lock)
            {
                var lockFactory = context.ServiceProvider.GetRequiredService<ILockerFactory>();
                var nameHelper = context.ServiceProvider.GetRequiredService<ICacheNamedHelper>();
                var lockResult = await LockHelper.GetLockAsync(context.InvocationInfo, lockFactory, nameHelper, LockTime);
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
        public override Task FindInMethodFinallyAsync<TResult>(AutoCacheDecoratorContext<TResult> context)
        {
            if (Lock)
            {
                if (context.Features.Contains(LockerKey) && context.Features[LockerKey] is ILocker locker)
                {
                    locker.Dispose();
                }
            }
            return base.FindInMethodFinallyAsync(context);
        }
        protected virtual Task GetLockFailAsync<TResult>(AutoCacheDecoratorContext<TResult> context, RunLockResult lockResult)
        {
            throw new GetLockFailException { locker = lockResult.Locker };
        }
        public override Task DecorateAsync<TResult>(AutoCacheDecoratorContext<TResult> context)
        {
            DefaultDataFinderOptions<UnwindObject, TResult> opt;
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
