using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public abstract class NamedInterceptor : AsyncInterceptorBase
    {
        protected abstract object GetLocker();

        protected abstract Dictionary<NamedInterceptorKey, NamedInterceptorValue> GetCacheMap();
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
        protected object[] MakeArgsWithHeader(NamedInterceptorValue value,object[] args)
        {
            var indexs = value.ArgIndexs;
            if (indexs == null)
            {
                var arr = new object[args.Length + 1];
                arr[0] = value.Header;
                Array.Copy(args, 0, arr, 1, args.Length);
                return arr;
            }
            var retArgs = new object[indexs.Count+1];
            retArgs[0] = value.Header;
            for (int i =1; i < indexs.Count; i++)
            {
                args[i] = args[indexs[i]];
            }
            return retArgs;
        }
        protected abstract bool ParamterCanUse(ParameterInfo param);
        protected NamedInterceptorValue GetArgIndexs(in NamedInterceptorKey key,MethodInfo method)
        {
            var map = GetCacheMap();
            var locker = GetLocker();
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
                //used.Count == methodArgs.Length
                IReadOnlyList<int> argIndexs = used.Count == methodArgs.Length ? null : used.ToArray();
                val = new NamedInterceptorValue(argIndexs, name);
                lock (locker)
                {
                    map[key] = val;
                }
            }
            return val;
        }
    }
}
