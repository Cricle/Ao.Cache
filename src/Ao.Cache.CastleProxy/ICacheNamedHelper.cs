using Ao.Cache.CastleProxy.Interceptors;

namespace Ao.Cache.CastleProxy
{
    public interface ICacheNamedHelper
    {
        NamedInterceptorValue GetArgIndexs(in NamedInterceptorKey key);
        IStringTransfer GetStringTransfer(in NamedInterceptorKey key);
        UnwindObject GetUnwindObject(in NamedInterceptorKey key, object[] args);
        UnwindObject GetUnwindObject(in NamedInterceptorKey key, object[] args, bool ignoreIndex);

        object[] MakeArgs(NamedInterceptorValue value, object[] args);
        object[] MakeArgsWithHeader(NamedInterceptorValue value, object[] args);
    }
}