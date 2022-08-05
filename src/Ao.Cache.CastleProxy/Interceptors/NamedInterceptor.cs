using Ao.Cache.CastleProxy.Annotations;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public abstract class NamedInterceptor : AsyncInterceptorBase,IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            InterceptSynchronous(invocation);
        }
    }
}
