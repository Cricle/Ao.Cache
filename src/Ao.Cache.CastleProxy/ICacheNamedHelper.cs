using Ao.Cache.CastleProxy.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ao.Cache.CastleProxy
{
    public interface ICacheNamedHelper
    {
        NamedInterceptorValue GetArgIndexs(in NamedInterceptorKey key);
        IStringTransfer GetStringTransfer(in NamedInterceptorKey key);
        UnwindObject GetUnwindObject(in NamedInterceptorKey key, object[] args);

        object[] MakeArgs(NamedInterceptorValue value, object[] args);
        object[] MakeArgsWithHeader(NamedInterceptorValue value, object[] args);
    }
}