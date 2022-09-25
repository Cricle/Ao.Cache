using Ao.Cache.Proxy;

/* 项目“Ao.Cache.MethodBoundaryAspect (netstandard2.0)”的未合并的更改
在此之前:
using System;
在此之后:
using MethodBoundaryAspect.Fody.Attributes;
using System;
*/
using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Reflection;
/* 项目“Ao.Cache.MethodBoundaryAspect (netstandard2.0)”的未合并的更改
在此之前:
using System.Threading.Tasks;
using MethodBoundaryAspect.Fody.Attributes;
在此之后:
using System.Threading.Tasks;
*/


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
