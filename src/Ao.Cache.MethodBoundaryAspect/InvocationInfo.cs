using Ao.Cache.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MethodBoundaryAspect.Fody.Attributes;

namespace Ao.Cache.MethodBoundaryAspect
{
    internal struct InvocationInfo : IInvocationInfo
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
