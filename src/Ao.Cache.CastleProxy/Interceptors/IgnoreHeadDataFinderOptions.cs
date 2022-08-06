namespace Ao.Cache.CastleProxy.Interceptors
{
    internal class IgnoreHeadDataFinderOptions<TResult>
    {
        public static readonly DefaultDataFinderOptions<UnwindObject, TResult> Options = new DefaultDataFinderOptions<UnwindObject, TResult>() { IgnoreHead = true };
    }
}
