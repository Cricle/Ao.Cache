namespace Ao.Cache.CastleProxy.Annotations
{
    public class AutoCacheResultBox<TResult>
    {
        private bool hasResult;
        private TResult result;

        public TResult Result => result;

        public bool HasResult => hasResult;

        public void SetResult(TResult result)
        {
            hasResult = true;
            this.result = result;
        }
    }
}
