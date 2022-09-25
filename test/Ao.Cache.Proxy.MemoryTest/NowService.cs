using Ao.Cache.Proxy.Annotations;

namespace Ao.Cache.Proxy.MemoryTest
{
    public class NowService
    {
        [AutoCache]
        [AutoCacheOptions("00:10:00")]
        public virtual Task<DateTime?> Now()
        {
            return Task.FromResult<DateTime?>(DateTime.Now);
        }
        [AutoCache]
        public virtual Task<DateTime?> NowWithArg(int a, [AutoCacheSkipPart] string b)
        {
            return Task.FromResult<DateTime?>(DateTime.Now);
        }
    }
    public class LockCache
    {
        [AutoCache]
        [AutoCacheOptions("00:01:00", Lock = true)]
        public virtual async Task<DateTime?> Now()
        {
            await Task.Yield();
            Console.WriteLine("命中方法啦！");
            return DateTime.Now;
        }
    }
    public class AddService
    {
        public virtual int Sum { get; set; }

        [AutoLock]
        public virtual async Task Add(int count)
        {
            await Task.Yield();
            for (int i = 0; i < count; i++)
            {
                Sum++;
            }
            await Task.Yield();
        }
    }
}
