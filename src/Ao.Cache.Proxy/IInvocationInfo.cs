using System;
using System.Reflection;

namespace Ao.Cache.Proxy
{
    public interface IInvocationInfo
    {
        object[] Arguments { get; }

        object Target { get; }

        MethodBase Method { get; }

        object ReturnValue { get; set; }

        Type TargetType { get; }
    }
}
