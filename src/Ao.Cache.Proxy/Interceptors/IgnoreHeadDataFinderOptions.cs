namespace Ao.Cache.Proxy.Interceptors
{
    internal class IgnoreHeadDataFinderOptions<TResult>
    {
        public static DefaultDataFinderOptions<UnwindObject, TResult> Options => new DefaultDataFinderOptions<UnwindObject, TResult>() { IgnoreHead = true };
    }
}
