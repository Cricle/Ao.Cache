using Ao.Cache.Proxy;
using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace Ao.Cache.CastleProxy
{
    internal class InvocationInfo : IInvocationInfo
    {
        public InvocationInfo(IInvocation invocation)
        {
            Invocation = invocation;
        }

        public IInvocation Invocation { get; }

        public object[] Arguments => Invocation.Arguments;

        public object Target => Invocation.TargetType;

        public MethodBase Method => Invocation.Method;

        public object ReturnValue
        {
            get => Invocation.ReturnValue;
            set => Invocation.ReturnValue = value;
        }

        public Type TargetType => Invocation.TargetType;
    }
}
