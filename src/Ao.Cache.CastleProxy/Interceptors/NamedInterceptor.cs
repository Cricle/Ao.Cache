using Castle.DynamicProxy;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public abstract class NamedInterceptor : AsyncInterceptorBase, IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            InterceptSynchronous(invocation);
        }
    }
}
