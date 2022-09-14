using Ao.Cache.Proxy;
using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Interceptors;
using Ao.Cache.Proxy.Model;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public class CacheInterceptor : AsyncInterceptorBase, IInterceptor
    {
        public CacheInterceptor(IServiceScopeFactory serviceScopeFactory,
            IStringTransfer stringTransfer,
            ICacheNamedHelper cacheNamedHelper)
        {
            ServiceScopeFactory = serviceScopeFactory;
            StringTransfer = stringTransfer;
            NamedHelper = cacheNamedHelper;
        }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public IStringTransfer StringTransfer { get; }

        public ICacheNamedHelper NamedHelper { get; }

        protected override Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            return proceed(invocation, proceedInfo);
        }
        class ActualTypeInfos
        {
            public Type ActualType;

            public Type FinderType;

            public bool TypesEquals;

            public Func<CacheInterceptor, IInvocationInfo, object, object> Method;
        }
        private static readonly object actionTypeLocker = new object();

        private static readonly Dictionary<Type, ActualTypeInfos> actualTypes = new Dictionary<Type, ActualTypeInfos>();
        private static ActualTypeInfos GetActionType(Type type)
        {
            if (!actualTypes.TryGetValue(type, out var t))
            {
                lock (actionTypeLocker)
                {
                    if (!actualTypes.TryGetValue(type, out t))
                    {
                        t = new ActualTypeInfos { ActualType = type, TypesEquals = true };
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(AutoCacheResult<>))
                        {
                            t.ActualType = type.GenericTypeArguments[0];
                            t.Method = CompileMethod(type, t.ActualType);
                            t.TypesEquals = false;
                        }
                        t.FinderType = typeof(IDataFinder<,>).MakeGenericType(typeof(UnwindObject), t.ActualType);
                        actualTypes[type] = t;
                    }
                }
            }
            return t;
        }
        private static Func<CacheInterceptor, IInvocationInfo, object, object> CompileMethod(Type prevType, Type actualType)
        {
            var coreInterceptMethod = typeof(InterceptLayout).GetMethod(nameof(InterceptLayout.RunAsync), 
                BindingFlags.Instance | BindingFlags.NonPublic);
            var method = coreInterceptMethod.MakeGenericMethod(actualType);
            var par0 = Expression.Parameter(typeof(InterceptLayout));
            var par1 = Expression.Parameter(typeof(IInvocationInfo));
            var par3 = Expression.Parameter(typeof(object));

            var par3Convert = Expression.Convert(par3,
                typeof(Func<,,>).MakeGenericType(typeof(IInvocation), typeof(Task<>).MakeGenericType(prevType)));

            var caseMethod = typeof(CacheInterceptor).GetMethod(nameof(Case), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(prevType, actualType);


            var par3Case = Expression.Call(null, caseMethod, par1, par3Convert);

            var body = Expression.Call(par0, method, par1, par3Case);
            return Expression.Lambda<Func<CacheInterceptor, IInvocationInfo, object, object>>(body,
                par0, par1, par3).Compile();
        }

        private static Func<Task<TOut>> Case<TResult, TOut>(IInvocationInfo invocation, Func<IInvocationInfo, Task<TResult>> proceed)
        {
            return async () =>
            {
                var res = await proceed(invocation).ConfigureAwait(false);
                if (res is AutoCacheResult<TOut> o)
                {
                    return o.RawData;
                }
                return (TOut)(object)res;
            };
        }
        
        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var originType = typeof(TResult);
            if (!AutoCacheAssertions.HasAutoCache(invocation.TargetType)&&
                !AutoCacheAssertions.HasAutoCache(invocation.Method))
            {
                var res = await proceed(invocation, proceedInfo).ConfigureAwait(false);
                if (res is IAutoCacheResult result)
                {
                    result.Status = AutoCacheStatus.Skip;
                }
                return res;
            }
            var key = new NamedInterceptorKey(invocation.TargetType, invocation.Method);
            var attr = DecoratorHelper.Get(key);
            var ctx = new AutoCacheInvokeDecoratorContext<TResult>(new InvocationInfo(invocation), ServiceScopeFactory);
            for (int i = 0; i < attr.Length; i++)
            {
                await attr[i].InterceptBeginAsync(ctx);
            }
            try
            {
                var actualTypeInfo = GetActionType(originType);
                if (actualTypeInfo.TypesEquals)
                {
                    var layout = new InterceptLayout(ServiceScopeFactory, NamedHelper);
                    var res = await layout.RunAsync(new InvocationInfo(invocation), () => proceed(invocation, proceedInfo)).ConfigureAwait(false);
                    var result = new AutoCacheInvokeResultContext<TResult>(res.RawData, res, null);
                    for (int i = 0; i < attr.Length; i++)
                    {
                        await attr[i].InterceptEndAsync(ctx, result);
                    }
                    return res.RawData;
                }
                var rr = await (Task<TResult>)actualTypeInfo.Method(this, new InvocationInfo(invocation), proceed);
                invocation.ReturnValue = rr;
                var cacheResult = new AutoCacheInvokeResultContext<TResult>(rr, rr as IAutoCacheResult, null);
                for (int i = 0; i < attr.Length; i++)
                {
                    await attr[i].InterceptEndAsync(ctx, cacheResult);
                }
                return rr;
            }
            catch (Exception ex)
            {
                for (int i = 0; i < attr.Length; i++)
                {
                    await attr[i].InterceptExceptionAsync(ctx, ex);
                }
                throw;
            }
            finally
            {
                for (int i = 0; i < attr.Length; i++)
                {
                    await attr[i].InterceptFinallyAsync(ctx);
                }
            }
        }

        public void Intercept(IInvocation invocation)
        {
            InterceptSynchronous(invocation);
        }
    }
}
