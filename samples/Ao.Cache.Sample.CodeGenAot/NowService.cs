using Ao.Cache.Annotations;

namespace Ao.Cache.Sample.CodeGenAot
{
    [CacheProxy]
    public class NowService
    {
        public virtual DateTime? NowSync(int? add, object? a, object? c)
        {
            return DateTime.Now.AddMilliseconds(add ?? 0);
        }
        public virtual ValueTask<DateTime?> Now(int? add,object? a,object? c)
        {
            return new ValueTask<DateTime?>(DateTime.Now.AddMilliseconds(add ?? 0));
        }
    }
}