using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Interceptors;
using Ao.Cache.Proxy.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy
{
    public class InterceptLayout
    {
        public InterceptLayout(IServiceScopeFactory serviceScopeFactory, ICacheNamedHelper namedHelper)
        {
            ServiceScopeFactory = serviceScopeFactory;
            NamedHelper = namedHelper;
        }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public ICacheNamedHelper NamedHelper { get; }

        public bool HasAutoCache(IInvocationInfo invocationInfo)
        {
            return AutoCacheAssertions.HasAutoCache(invocationInfo.TargetType) ||
                 AutoCacheAssertions.HasAutoCache(invocationInfo.Method);
        }

        public class InterceptToken<TResult> : IDisposable
        {
            public InterceptToken(IInvocationInfo invocationInfo, InterceptLayout layout)
            {
                InvocationInfo = invocationInfo;
                Layout = layout;
                Key = new NamedInterceptorKey(invocationInfo.TargetType, invocationInfo.Method);
                Attributes = DecoratorHelper.Get(Key);
                Context = new AutoCacheInvokeDecoratorContext<TResult>(invocationInfo, layout.ServiceScopeFactory);
                ActualTypeInfos = ActionTypeHelper.GetActionType(typeof(TResult));
                Result = new AutoCacheResult<TResult>();
                Scope = ServiceScopeFactory.CreateScope();
                DataFinderFactory = Scope.ServiceProvider.GetRequiredService<IDataFinderFactory>();
                DataFinder = DataFinderFactory.CreateEmpty<UnwindObject, TResult>();
            }

            private UnwindObject? unwindObject;
            private AutoCacheDecoratorContext<TResult> autoCacheDecoratorContext;
            private AutoCacheResultBox<TResult> autoCacheResultBox;

            public AutoCacheResultBox<TResult> AutoCacheResultBox
            {
                get
                {
                    if (autoCacheResultBox == null)
                    {
                        autoCacheResultBox = new AutoCacheResultBox<TResult>();
                    }
                    return autoCacheResultBox;
                }
            }

            public AutoCacheDecoratorContext<TResult> AutoCacheDecoratorContext
            {
                get
                {
                    if (autoCacheDecoratorContext == null)
                    {
                        autoCacheDecoratorContext = new AutoCacheDecoratorContext<TResult>(
                            InvocationInfo, Scope.ServiceProvider, DataFinder, UnwindObject);
                    }
                    return autoCacheDecoratorContext;
                }
            }

            public UnwindObject UnwindObject
            {
                get
                {
                    if (unwindObject == null)
                    {
                        unwindObject = Layout.NamedHelper.GetUnwindObject(Key, InvocationInfo.Arguments);
                    }
                    return unwindObject.Value;
                }
            }

            public IDataFinderFactory DataFinderFactory { get; }

            public IDataFinder<UnwindObject, TResult> DataFinder { get; }

            public IInvocationInfo InvocationInfo { get; }

            public NamedInterceptorKey Key { get; }

            public InterceptLayout Layout { get; }

            public AutoCacheDecoratorBaseAttribute[] Attributes { get; }

            public IServiceScopeFactory ServiceScopeFactory => Layout.ServiceScopeFactory;

            public AutoCacheInvokeDecoratorContext<TResult> Context { get; }

            public ActualTypeInfos ActualTypeInfos { get; }

            public IServiceScope Scope { get; }

            public AutoCacheResult<TResult> Result { get; }

            public async Task InterceptBeginAsync()
            {
                for (int i = 0; i < Attributes.Length; i++)
                {
                    await Attributes[i].InterceptBeginAsync(Context);
                }
            }

            public async Task InterceptExceptionAsync(Exception exception)
            {
                for (int i = 0; i < Attributes.Length; i++)
                {
                    await Attributes[i].InterceptExceptionAsync(Context, exception);
                }
            }
            public async Task FinallyAsync()
            {
                for (int i = 0; i < Attributes.Length; i++)
                {
                    await Attributes[i].InterceptFinallyAsync(Context);
                }
            }
            public async Task InterceptEndAsync(AutoCacheResult<TResult> result)
            {
                var cacheResult = new AutoCacheInvokeResultContext<TResult>(result.RawData, result, null);
                for (int i = 0; i < Attributes.Length; i++)
                {
                    await Attributes[i].InterceptEndAsync(Context, cacheResult);
                }
            }
            public async Task DecorateAsync()
            {
                for (int i = 0; i < Attributes.Length; i++)
                {
                    await Attributes[i].DecorateAsync(AutoCacheDecoratorContext);
                }
            }
            public async Task FindInMethodBeginAsync()
            {
                for (int i = 0; i < Attributes.Length; i++)
                {
                    await Attributes[i].FindInMethodBeginAsync(AutoCacheDecoratorContext, AutoCacheResultBox);
                }
                if (AutoCacheResultBox.HasResult)
                {
                    Result.RawData = AutoCacheResultBox.Result;
                    Result.Status = AutoCacheStatus.Intercept;
                }
            }
            public async Task FindInMethodEndAsync()
            {
                if (AutoCacheResultBox.HasResult)
                {
                    await DataFinder.SetInCacheAsync(UnwindObject, AutoCacheResultBox.Result).ConfigureAwait(false);
                }
                Result.Status = AutoCacheStatus.MethodHit;
                Result.RawData = AutoCacheResultBox.Result;
                for (int i = 0; i < Attributes.Length; i++)
                {
                    await Attributes[i].FindInMethodEndAsync(AutoCacheDecoratorContext, AutoCacheResultBox.Result, AutoCacheResultBox.HasResult);
                }
            }
            public async Task FoundInCacheAsync(TResult result)
            {
                Result.Status = AutoCacheStatus.CacheHit;
                Result.RawData = result;
                InvocationInfo.ReturnValue = result;
                for (int i = 0; i < Attributes.Length; i++)
                {
                    await Attributes[i].FoundInCacheAsync(AutoCacheDecoratorContext, result);
                }
            }
            public async Task FindInMethodFinallyAsync()
            {
                for (int i = 0; i < Attributes.Length; i++)
                {
                    await Attributes[i].FindInMethodFinallyAsync(AutoCacheDecoratorContext);
                }
            }

            public async Task<TResult> TryFindInCacheAsync()
            {
                await DecorateAsync();
                return await DataFinder.FindInCacheAsync(UnwindObject).ConfigureAwait(false);
            }

            public void Dispose()
            {
                Scope.Dispose();
            }
        }

        public InterceptToken<TResult> CreateToken<TResult>(IInvocationInfo invocationInfo)
        {
            return new InterceptToken<TResult>(invocationInfo, this);
        }
        //public async Task<AutoCacheResult<TResult>> RunAsync<TResult>(IInvocationInfo invocationInfo, Func<Task<TResult>> proceed)
        //{
        //    var actualType = typeof(TResult).GetGenericArguments()[0];
        //    var method = typeof(InterceptLayout).GetMethod(nameof(RunAsync))
        //        .MakeGenericMethod(actualType);
        //}
        public async Task<AutoCacheResult<TResult>> RunAsync<TResult>(IInvocationInfo invocationInfo, Func<Task<TResult>> proceed)
        {
            using (var token = CreateToken<TResult>(invocationInfo))
            {
                if (await token.TryFindInCacheAsync() == null)
                {
                    await token.FindInMethodBeginAsync();
                    if (!token.AutoCacheResultBox.HasResult)
                    {
                        try
                        {
                            var res = await proceed();
                            token.AutoCacheResultBox.SetResult(res);
                            await token.FindInMethodEndAsync();
                        }
                        finally
                        {
                            await token.FindInMethodFinallyAsync();
                        }
                    }
                }
                return token.Result;
            }
        }
    }
}
