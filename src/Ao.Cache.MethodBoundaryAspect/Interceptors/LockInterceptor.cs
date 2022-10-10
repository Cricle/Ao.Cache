using Ao.Cache.Proxy;
using Ao.Cache.Proxy.Exceptions;
using Ao.Cache.Proxy.Interceptors;
using MethodBoundaryAspect.Fody.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Ao.Cache.MethodBoundaryAspect.Interceptors
{
    public class LockInterceptor : OnMethodBoundaryAspect, IAsyncMethodHandle
    {
        private IServiceScope scope;
        private RunLockResult? result;

        public override void OnEntry(MethodExecutionArgs arg)
        {
            MethodBoundaryAspectHelper.AsyncIntercept(arg, this, MethodBoundaryMethods.Entry, MethodReturnCase.Task| MethodReturnCase.TaskResult);
        }
        public override void OnExit(MethodExecutionArgs arg)
        {
            MethodBoundaryAspectHelper.AsyncIntercept(arg, this, MethodBoundaryMethods.Exit, MethodReturnCase.Task | MethodReturnCase.TaskResult);
            scope?.Dispose();
            result?.Dispose();
        }
        public override void OnException(MethodExecutionArgs arg)
        {
            MethodBoundaryAspectHelper.AsyncIntercept(arg, this, MethodBoundaryMethods.Exception, MethodReturnCase.Task | MethodReturnCase.TaskResult);
        }
        public async Task<T> HandleEntryAsync<T>(MethodExecutionArgs arg, T old)
        {
            scope = GlobalMethodBoundary.CreateScope();
            var lockerFactory = scope.ServiceProvider.GetRequiredService<ILockerFactory>();
            var namedHelper = scope.ServiceProvider.GetRequiredService<ICacheNamedHelper>();
            var r = await LockHelper.GetLockAsync(new InvocationInfo(arg), lockerFactory, namedHelper);
            result = r;
            if (r.Locker != null)
            {
                if (r.Type != RunLockResultTypes.InLocker ||
                    !r.Locker.IsAcquired)
                {
                    await GetLockFailAsync(arg, r);
                }
            }
            return old;
        }
        protected virtual Task GetLockFailAsync(MethodExecutionArgs arg, RunLockResult result)
        {
            throw new GetLockFailException { Locker = result.Locker };
        }
        public Task<T> HandleExceptionAsync<T>(MethodExecutionArgs arg, T old)
        {
            return Task.FromResult(old);
        }

        public Task<T> HandleExitAsync<T>(MethodExecutionArgs arg, T old)
        {
            return Task.FromResult(old);
        }
    }
}
