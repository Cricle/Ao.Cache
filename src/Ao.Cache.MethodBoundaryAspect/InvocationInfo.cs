using Ao.Cache.Proxy;
using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Reflection;

namespace Ao.Cache.MethodBoundaryAspect
{
    internal class InvocationInfo : IInvocationInfo
    {
        private readonly MethodExecutionArgs arg;

        public InvocationInfo(MethodExecutionArgs arg)
        {
            this.arg = arg ?? throw new ArgumentNullException(nameof(arg));
        }

        public object[] Arguments => arg.Arguments;

        public object Target => arg.Instance;

        public MethodBase Method => arg.Method;

        public object ReturnValue
        {
            get => arg.ReturnValue;
            set => arg.ReturnValue = value;
        }

        public Type TargetType => arg?.Instance.GetType();
    }
}
