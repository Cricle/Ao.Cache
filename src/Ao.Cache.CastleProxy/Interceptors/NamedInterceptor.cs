using Ao.Cache.CastleProxy.Annotations;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public abstract class NamedInterceptor : AsyncInterceptorBase
    {
        protected abstract object GetLocker();

        protected abstract Dictionary<NamedInterceptorKey, NamedInterceptorValue> CacheMap { get; }

        protected object[] MakeArgs(NamedInterceptorValue value, object[] args)
        {
            var indexs = value.ArgIndexs;
            if (indexs == null)
            {
                return args;
            }
            var retArgs = new object[indexs.Count];
            for (int i = 0; i < indexs.Count; i++)
            {
                args[i] = args[indexs[i]];
            }
            return retArgs;
        }

        protected virtual object[] MakeArgsWithHeader(NamedInterceptorValue value,object[] args)
        {
            var indexs = value.ArgIndexs;
            if (indexs == null)
            {
                return args;
            }
            var retArgs = new object[indexs.Count];
            for (int i = 0; i < indexs.Count; i++)
            {
                retArgs[i] = args[indexs[i]];
            }
            return retArgs;
        }
        protected abstract bool ParamterCanUse(ParameterInfo param);
        protected virtual IStringTransfer GetDefaultStringTransfer(in NamedInterceptorKey key)
        {
            return DefaultStringTransfer.Default;
        }
        protected IStringTransfer GetStringTransfer(in NamedInterceptorKey key)
        {
            var attr = key.Method.GetCustomAttribute<StringTransferAttribute>();
            if (attr != null)
            {
                return (IStringTransfer)Activator.CreateInstance(attr.StringTransferType);
            }
            attr= key.TargetType.GetCustomAttribute<StringTransferAttribute>();
            if (attr != null)
            {
                return (IStringTransfer)Activator.CreateInstance(attr.StringTransferType);
            }
            return GetDefaultStringTransfer(key);
        }
        protected NamedInterceptorValue GetArgIndexs(in NamedInterceptorKey key,MethodInfo method)
        {
            var map = CacheMap;
            if (!map.TryGetValue(key, out var val))
            {
                var methodArgs = method.GetParameters();
                var used = new List<int>();
                for (int i = 0; i < methodArgs.Length; i++)
                {
                    var methodArg = methodArgs[i];
                    if (ParamterCanUse(methodArg))
                    {
                        used.Add(i);
                    }
                }
                var name = TypeNameHelper.GetFriendlyFullName(key.TargetType) + "." + key.Method.Name;
                IReadOnlyList<int> argIndexs = used.Count == methodArgs.Length ? null : used.ToArray();
                var sf = GetStringTransfer(key);
                val = new NamedInterceptorValue(argIndexs,sf, name);
                var locker = GetLocker();
                lock (locker)
                {
                    map[key] = val;
                }
            }
            return val;
        }
    }
}
