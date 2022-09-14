using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Interceptors;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.Cache.Proxy
{
    public class DefaultCacheNamedHelper : ICacheNamedHelper
    {
        public static readonly DefaultCacheNamedHelper Default = new DefaultCacheNamedHelper(DefaultStringTransfer.Default);

        public object SyncRoot { get; } = new object();

        private readonly Dictionary<NamedInterceptorKey, NamedInterceptorValue> cacheMap = new Dictionary<NamedInterceptorKey, NamedInterceptorValue>();

        public DefaultCacheNamedHelper(IStringTransfer stringTransfer)
        {
            StringTransfer = stringTransfer ?? throw new ArgumentNullException(nameof(stringTransfer));
        }

        public IStringTransfer StringTransfer { get; }

        public object[] MakeArgs(NamedInterceptorValue value, object[] args)
        {
            var indexs = value.ArgIndexs;
            if (indexs == null)
            {
                return args;
            }
            var retArgs = ArrayMaker.Make<object>(indexs.Count);
            for (int i = 0; i < indexs.Count; i++)
            {
                retArgs[i] = args[indexs[i]];
            }
            return retArgs;
        }

        public object[] MakeArgsWithHeader(NamedInterceptorValue value, object[] args)
        {
            var indexs = value.ArgIndexs;
            if (indexs == null)
            {
                var arr = ArrayMaker.Make<object>(args.Length + 1);
                arr[0] = value.Header;
                Array.Copy(args, 0, arr, 1, args.Length);
                return arr;
            }
            var retArgs = ArrayMaker.Make<object>(indexs.Count + 1);
            retArgs[0] = value.Header;
            for (int i = 0; i < indexs.Count; i++)
            {
                retArgs[i + 1] = args[indexs[i]];
            }
            return retArgs;
        }
        protected virtual bool ParamterCanUse(ParameterInfo param)
        {
            return param.GetCustomAttribute<KeySkipPartAttribute>() == null;
        }
        public UnwindObject GetUnwindObject(in NamedInterceptorKey key, object[] args)
        {
            return GetUnwindObject(key, args, false);
        }
        public UnwindObject GetUnwindObject(in NamedInterceptorKey key, object[] args, bool ignoreIndex)
        {
            var lst = GetArgIndexs(key);
            if (ignoreIndex)
            {
                return new UnwindObject(lst.Header, args, StringTransfer);
            }
            var objs = MakeArgs(lst, args);
            return new UnwindObject(lst.Header, objs, StringTransfer);
        }
        protected virtual IStringTransfer GetDefaultStringTransfer(in NamedInterceptorKey key)
        {
            return DefaultStringTransfer.Default;
        }
        public IStringTransfer GetStringTransfer(in NamedInterceptorKey key)
        {
            var attr = key.Method.GetCustomAttribute<StringTransferAttribute>();
            if (attr != null)
            {
                return (IStringTransfer)Activator.CreateInstance(attr.StringTransferType);
            }
            attr = key.TargetType.GetCustomAttribute<StringTransferAttribute>();
            if (attr != null)
            {
                return (IStringTransfer)Activator.CreateInstance(attr.StringTransferType);
            }
            return GetDefaultStringTransfer(key);
        }

        public NamedInterceptorValue GetArgIndexs(in NamedInterceptorKey key)
        {
            if (!cacheMap.TryGetValue(key, out var val))
            {
                lock (SyncRoot)
                {
                    if (!cacheMap.TryGetValue(key, out val))
                    {
                        var methodArgs = key.Method.GetParameters();
                        var used = new List<int>(methodArgs.Length);
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
                        val = new NamedInterceptorValue(argIndexs, sf, name);
                        cacheMap[key] = val;
                    }
                }
            }
            return val;
        }

    }
}
