using Ao.Cache.Proxy;
using Ao.Cache.Proxy.Model;
using MethodBoundaryAspect.Fody.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Ao.Cache.MethodBoundaryAspect.Interceptors
{
    public class CacheInterceptor : OnMethodBoundaryAspect, IAsyncMethodHandle
    {
        private IServiceScope scope;
        private InterceptLayout layout;
        private InvocationInfo ii;
        private object token;

        public async Task<T> HandleEntryAsync<T>(MethodExecutionArgs arg, T old)
        {
            scope = GlobalMethodBoundary.CreateScope();
            var cacheNamedHelper = scope.ServiceProvider.GetRequiredService<ICacheNamedHelper>();
            ii = new InvocationInfo(arg);
            if (!InterceptLayout.HasAutoCache(ii))
            {
                if (((MethodInfo)arg.Method).ReturnType is IAutoCacheResult result)
                {
                    result.Status = AutoCacheStatus.Skip;
                }
                return old;
            }
            layout = new InterceptLayout(GlobalMethodBoundary.ServiceScopeFactory, cacheNamedHelper);

            var token = layout.CreateToken<T>(ii, scope);
            this.token = token;
            await token.InterceptBeginAsync();
            var cacheRes = await token.TryFindInCacheAsync().ConfigureAwait(false);
            if (cacheRes == null)
            {
                await token.FindInMethodBeginAsync();
                if (token.AutoCacheResultBox.HasResult)
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

        public async Task<T> HandleExceptionAsync<T>(MethodExecutionArgs arg, T old)
        {
            var tk = (InterceptToken<T>)token;
            await tk.InterceptExceptionAsync(arg.Exception);
            await tk.FindInMethodFinallyAsync();
            await tk.FinallyAsync();
            return old;
        }
        private static bool TryGetValue<T>(InterceptToken<T> token, out object res)
        {
            if (CacheResultNewExpression<T>.IsAutoResult)
            {
                var dyn = (IAutoCacheResult)CacheResultNewExpression<T>.Creator();
                dyn.Status = token.Result.Status;
                var d = token.AutoCacheResultBox.Result;
                if (d != null)
                {
                    dyn.RawData = ((IAutoCacheResult)d).RawData;
                }
                res = dyn;
                return true;
            }
            res = default(T);
            return false;
        }
        public async Task<T> HandleExitAsync<T>(MethodExecutionArgs arg, T old)
        {
            var tk = (InterceptToken<T>)token;
            tk.AutoCacheResultBox.SetResult(old);
            await tk.FindInMethodEndAsync().ConfigureAwait(false);
            await tk.FinallyAsync().ConfigureAwait(false);
            if (TryGetValue(tk, out var res))
            {
                return (T)res;
            }
            return tk.AutoCacheResultBox.Result;
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
            scope?.Dispose();
            (token as IDisposable)?.Dispose();
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
