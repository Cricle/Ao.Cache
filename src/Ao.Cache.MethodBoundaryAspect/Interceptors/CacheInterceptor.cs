using Ao.Cache.Proxy;
using Ao.Cache.Proxy.Model;
using MethodBoundaryAspect.Fody.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Ao.Cache.MethodBoundaryAspect.Interceptors
{
    public class CacheInterceptor : OnMethodBoundaryAspect, IAsyncMethodHandle
    {
        private readonly IServiceScope scope;
        private readonly IStringTransfer stringTransfer;
        private readonly ICacheNamedHelper cacheNamedHelper;
        private InterceptLayout layout;
        private InvocationInfo ii;
        private dynamic token;

        private bool skip;

        public CacheInterceptor()
        {
            scope = GlobalMethodBoundary.CreateScope();
            stringTransfer = scope.ServiceProvider.GetRequiredService<IStringTransfer>();
            cacheNamedHelper = scope.ServiceProvider.GetRequiredService<ICacheNamedHelper>();
        }

        public async Task<T> HandleEntryAsync<T>(MethodExecutionArgs arg, T old)
        {
            ii = new InvocationInfo(arg);
            layout = new InterceptLayout(GlobalMethodBoundary.ServiceScopeFactory, cacheNamedHelper);
            if (!layout.HasAutoCache(ii))
            {
                skip = true;
                if (((MethodInfo)arg.Method).ReturnType is IAutoCacheResult result)
                {
                    result.Status = AutoCacheStatus.Skip;
                }
                return old;
            }

            var token = layout.CreateToken<T>(ii);
            this.token = token;
            await token.InterceptBeginAsync();
            var cacheRes = await token.TryFindInCacheAsync();
            if (cacheRes == null)
            {
                await token.FindInMethodBeginAsync();
                if (!token.AutoCacheResultBox.HasResult)
                {
                    arg.FlowBehavior = FlowBehavior.Continue;
                }
                else
                {
                    arg.FlowBehavior = FlowBehavior.Return;
                }
            }
            else
            {
                arg.FlowBehavior = FlowBehavior.Return;
                return cacheRes;
            }
            return old;
        }

        static class NewExpression<T>
        {
            public static readonly Func<T> Creator;

            public static bool IsAutoResult;

            static NewExpression()
            {
                var typeT = typeof(T);
                IsAutoResult = typeT.IsGenericType &&
                    typeT.GetGenericTypeDefinition() == typeof(AutoCacheResult<>);
                Creator = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
            }
        }

        public async Task<T> HandleExceptionAsync<T>(MethodExecutionArgs arg, T old)
        {
            await token.InterceptExceptionAsync(arg.Exception);
            await token.FindInMethodFinallyAsync();
            await token.FinallyAsync();
            return old;
        }
        private bool TryGetValue<T>(out dynamic res)
        {
            if (NewExpression<T>.IsAutoResult)
            {
                dynamic dyn = NewExpression<T>.Creator();
                dyn.Status = token.Result.Status;
                var d = token.AutoCacheResultBox.Result;
                if (d != null)
                {
                    dyn.RawData = ((dynamic)token.AutoCacheResultBox.Result).RawData;
                }
                res= dyn;
                return true;
            }
            res = default(T);
            return false;
        }
        public async Task<T> HandleExitAsync<T>(MethodExecutionArgs arg, T old)
        {            
            token.AutoCacheResultBox.SetResult(old);
            await token.FindInMethodEndAsync();
            await token.FinallyAsync();
            if (TryGetValue<T>(out var res))
            {
                return res;
            }
            return token.Result.RawData;
        }

        public override void OnEntry(MethodExecutionArgs arg)
        {
            MethodBoundaryAspectHelper.AsyncIntercept(arg, this, MethodBoundaryMethods.Entry);
        }

        public override void OnExit(MethodExecutionArgs arg)
        {
            if (token != null)
            {
                MethodBoundaryAspectHelper.AsyncIntercept(arg, this, MethodBoundaryMethods.Exit);
            }
            scope.Dispose();
            token?.Dispose();
        }
        public override void OnException(MethodExecutionArgs arg)
        {
            if (token != null)
            {
                MethodBoundaryAspectHelper.AsyncIntercept(arg, this, MethodBoundaryMethods.Exception);
            }
        }
    }
}
