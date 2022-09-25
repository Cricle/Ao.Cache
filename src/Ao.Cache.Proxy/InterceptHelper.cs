using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Interceptors;
using Ao.Cache.Proxy.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

        public bool HasAutoCache(IInvocationInfo invocationInfo)
        {
            return AutoCacheAssertions.HasAutoCache(invocationInfo.TargetType) ||
                 AutoCacheAssertions.HasAutoCache(invocationInfo.Method);
        }

        public InterceptToken<TResult> CreateToken<TResult>(IInvocationInfo invocationInfo, IServiceScope scope = null)
        {
            return new InterceptToken<TResult>(invocationInfo, this, scope);
        }
    }
}
