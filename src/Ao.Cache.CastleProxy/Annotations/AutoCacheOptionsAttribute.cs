using Ao.Cache.CastleProxy.Interceptors;
using Castle.DynamicProxy;
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
        public static readonly TimeSpan? DefaultCacheTime = TimeSpan.FromSeconds(3);

        public AutoCacheOptionsAttribute()
        {
        }

        public AutoCacheOptionsAttribute(string cacheTimeStr)
        {
            CacheTime = TimeSpan.Parse(cacheTimeStr);
        }

        public bool CanRenewal { get; set; }

        public TimeSpan? CacheTime { get; } = DefaultCacheTime;

        public bool Renewal { get; set; }

        public override Task DecorateAsync<TResult>(AutoCacheDecoratorContext<TResult> context)
        {
            DefaultDataFinderOptions<UnwindObject,TResult> opt = null;
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
