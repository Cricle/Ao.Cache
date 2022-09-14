﻿using Ao.Cache.Proxy.Interceptors;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AutoCacheAttribute : AutoCacheDecoratorBaseAttribute
    {
        public AutoCacheAttribute()
        {
            Order = int.MinValue;
        }

        public override Task DecorateAsync<TResult>(AutoCacheDecoratorContext<TResult> context)
        {
            context.DataFinder.Options = IgnoreHeadDataFinderOptions<TResult>.Options;
            return base.DecorateAsync(context);
        }
    }
}