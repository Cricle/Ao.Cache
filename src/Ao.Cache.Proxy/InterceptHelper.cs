using Ao.Cache.Proxy.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ao.Cache.Proxy
{
    public readonly struct InterceptLayout
    {
        public InterceptLayout(IServiceScopeFactory serviceScopeFactory, ICacheNamedHelper namedHelper)
        {
            ServiceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            NamedHelper = namedHelper ?? throw new ArgumentNullException(nameof(namedHelper));
        }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public ICacheNamedHelper NamedHelper { get; }

        public static bool HasAutoCache(IInvocationInfo invocationInfo)
        {
            return AutoCacheAssertions.HasAutoCache(invocationInfo.Method) ||
                AutoCacheAssertions.HasAutoCache(invocationInfo.TargetType);
        }

        public InterceptToken<TResult> CreateToken<TResult>(IInvocationInfo invocationInfo, IServiceScope scope = null)
        {
            return new InterceptToken<TResult>(invocationInfo, this, scope);
        }
    }
}
