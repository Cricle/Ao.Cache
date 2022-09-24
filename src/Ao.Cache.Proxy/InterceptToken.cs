using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Interceptors;
using Ao.Cache.Proxy.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy
{
    public class InterceptToken<TResult> : IDisposable
    {
        private static readonly Type ResultType = typeof(TResult);
        public static readonly ActualTypeInfos ActualTypeInfos = ActionTypeHelper.GetActionType(ResultType);

        public InterceptToken(IInvocationInfo invocationInfo, InterceptLayout layout, IServiceScope scope = null)
        {
            InvocationInfo = invocationInfo ?? throw new ArgumentNullException(nameof(invocationInfo));
            Layout = layout;
            Key = new NamedInterceptorKey(invocationInfo.TargetType, invocationInfo.Method);
            Attributes = DecoratorHelper.Get(Key);
            Context = new AutoCacheInvokeDecoratorContext<TResult>(invocationInfo, layout.ServiceScopeFactory);
            Result = new AutoCacheResult<TResult>();
            this.scope = scope ?? ServiceScopeFactory.CreateScope();
            DataFinderFactory = this.scope.ServiceProvider.GetRequiredService<IDataFinderFactory>();
            DataFinder = DataFinderFactory.CreateEmpty<UnwindObject, TResult>();
        }
        private IServiceScope scope;
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

        public IServiceScope Scope => scope;

        public AutoCacheResult<TResult> Result { get; }

        public async Task InterceptBeginAsync()
        {
            for (int i = 0; i < Attributes.Length; i++)
            {
                await Attributes[i].InterceptBeginAsync(Context).ConfigureAwait(false);
            }
        }

        public async Task InterceptExceptionAsync(Exception exception)
        {
            for (int i = 0; i < Attributes.Length; i++)
            {
                await Attributes[i].InterceptExceptionAsync(Context, exception).ConfigureAwait(false);
            }
        }
        public async Task FinallyAsync()
        {
            for (int i = 0; i < Attributes.Length; i++)
            {
                await Attributes[i].InterceptFinallyAsync(Context).ConfigureAwait(false);
            }
        }
        public async Task InterceptEndAsync(AutoCacheResult<TResult> result)
        {
            var cacheResult = new AutoCacheInvokeResultContext<TResult>(result.RawData, result, null);
            for (int i = 0; i < Attributes.Length; i++)
            {
                await Attributes[i].InterceptEndAsync(Context, cacheResult).ConfigureAwait(false);
            }
        }
        public async Task DecorateAsync()
        {
            for (int i = 0; i < Attributes.Length; i++)
            {
                await Attributes[i].DecorateAsync(AutoCacheDecoratorContext).ConfigureAwait(false);
            }
        }
        public async Task FindInMethodBeginAsync()
        {
            for (int i = 0; i < Attributes.Length; i++)
            {
                await Attributes[i].FindInMethodBeginAsync(AutoCacheDecoratorContext, AutoCacheResultBox).ConfigureAwait(false);
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
                await Attributes[i].FindInMethodEndAsync(AutoCacheDecoratorContext, AutoCacheResultBox.Result, AutoCacheResultBox.HasResult).ConfigureAwait(false);
            }
        }
        public async Task FoundInCacheAsync(TResult result)
        {
            Result.Status = AutoCacheStatus.CacheHit;
            Result.RawData = result;
            InvocationInfo.ReturnValue = result;
            for (int i = 0; i < Attributes.Length; i++)
            {
                await Attributes[i].FoundInCacheAsync(AutoCacheDecoratorContext, result).ConfigureAwait(false);
            }
        }
        public async Task FindInMethodFinallyAsync()
        {
            for (int i = 0; i < Attributes.Length; i++)
            {
                await Attributes[i].FindInMethodFinallyAsync(AutoCacheDecoratorContext).ConfigureAwait(false);
            }
        }

        public async Task<TResult> TryFindInCacheAsync()
        {
            await DecorateAsync().ConfigureAwait(false);
            return await DataFinder.FindInCacheAsync(UnwindObject).ConfigureAwait(false);
        }

        public void Dispose()
        {
            Scope.Dispose();
        }
    }
}
