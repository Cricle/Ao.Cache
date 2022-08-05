namespace Ao.Cache.CastleProxy.Interceptors
{
    internal class IgnoreHeadDataFinderOptions<TResult>
    {
        public static readonly IDataFinderOptions<UnwindObject, TResult> Options = new DefaultDataFinderOptions<UnwindObject, TResult>() { IgnoreHead = true };
    }
}
