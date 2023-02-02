using System.Runtime.CompilerServices;

namespace Ao.Cache.Proxy.Annotations
{
    public class AutoCacheResultBox<TResult>
    {
        internal bool hasResult;
        private TResult result;

        public TResult Result => result;

        public bool HasResult => hasResult;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult(TResult result)
        {
            hasResult = true;
            this.result = result;
        }
    }
}
