using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.CastleProxy.Model;
using Ao.Cache.Serizlier.MessagePack;
using Ao.Cache.Serizlier.SpanJson;
using Castle.DynamicProxy;
using DryIoc;
using DryIoc.ImTools;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Structing.DryInterceptor;
using Structing.DryInterceptor.Annotations;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Ao.Cache.CastleProxy.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ser = new ServiceCollection();
            ser.AddSingleton<GetTime>();
            ser.AddSingleton<LockTime>();
            ser.AddSingleton<LockCache>();
            ser.AddSingleton<IGetTime,NoGetTime>();
            ser.AddCastleCacheProxy();
            var s = ConfigurationOptions.Parse("127.0.0.1:6379");
            ser.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(s));
            ser.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            ser.AddDistributedLockFactory();
            ser.AddSingleton<IEntityConvertor, SpanJsonEntityConvertor>();
            ser.AddSingleton(typeof(IEntityConvertor<>), typeof(SpanJsonEntityConvertor<>));
            //ser.AddInMemoryFinder();
            ser.AddInRedisFinder();

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            icon.AsyncIntercept<GetTime, CacheInterceptor>();
            icon.AsyncIntercept<LockCache, CacheInterceptor>();
            icon.AsyncIntercept<LockTime, LockInterceptor>();
            icon.AsyncIntercept<IGetTime, CacheInterceptor>();
            var provider = icon.BuildServiceProvider();
            var scope = provider.CreateScope();
            var sw = Stopwatch.GetTimestamp();
            RunCache(scope.ServiceProvider).GetAwaiter().GetResult();
            ShowDescript($"耗时:{new TimeSpan(Stopwatch.GetTimestamp() - sw)}");
            sw = Stopwatch.GetTimestamp();
            RunCacheWithStatus(scope.ServiceProvider).GetAwaiter().GetResult();
            ShowDescript($"耗时:{new TimeSpan(Stopwatch.GetTimestamp() - sw)}");
            sw = Stopwatch.GetTimestamp();
            RunLock(scope.ServiceProvider).GetAwaiter().GetResult();
            ShowDescript($"耗时:{new TimeSpan(Stopwatch.GetTimestamp() - sw)}");
            sw = Stopwatch.GetTimestamp();
            RunLockCache(scope.ServiceProvider).GetAwaiter().GetResult();
            ShowDescript($"耗时:{new TimeSpan(Stopwatch.GetTimestamp() - sw)}");
            sw = Stopwatch.GetTimestamp();
            InterfaceRunCache(scope.ServiceProvider).GetAwaiter().GetResult();
            ShowDescript($"耗时:{new TimeSpan(Stopwatch.GetTimestamp() - sw)}");
        }
        private static void ShowTitle(string text)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        private static void ShowDescript(string text)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        private static async Task RunLockCache(IServiceProvider provider)
        {
            ShowTitle("Run lock cache");
            var gt = provider.GetRequiredService<LockCache>();
            var finderFc = provider.GetRequiredService<AutoCacheService>();
            await finderFc.DeleteAsync<LockCache, DateTime?>(x => x.Now());//Clear up
            var tasks = new Task[10];
            var times = new ConcurrentBag<DateTime>();
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] =await Task.Factory.StartNew(async () =>
                {
                    var d = await gt.Now();
                    times.Add(d.Value);
                });
            }
            await Task.WhenAll(tasks);
            var group = times.GroupBy(x => x).Count();
            if (group != 1)
            {
                Console.WriteLine("失败");
                foreach (var item in times.GroupBy(x => x))
                {
                    Console.WriteLine($"{item.Key:yyyy-MM-dd HH:mm:ss.ffff} = {item.Count()}");
                }
            }
            Console.WriteLine(group);
        }
        private static async Task InterfaceRunCache(IServiceProvider provider)
        {
            ShowTitle("Run interface cache");
            var gt = provider.GetRequiredService<IGetTime>();
            var finderFc = provider.GetRequiredService<AutoCacheService>();
            await finderFc.DeleteAsync<IGetTime, DtObj>(gt, x => x.NowTime1(1, 0));//Clear up
            var r1 = await gt.NowTime1(1, 0);
            Console.WriteLine($"First:{r1.Time:yyyy:MM:dd HH:mm:ss.ffff}");
            var r2 = await gt.NowTime1(1, 0);
            Console.WriteLine($"Second:{r1.Time:yyyy:MM:dd HH:mm:ss.ffff}");
            var delRes = await finderFc.DeleteAsync<IGetTime, DtObj>(gt, x => x.NowTime1(1, 0));
            Console.WriteLine($"Deleted Result:{delRes}");
            var r3 = await gt.NowTime1(1, 0);
            Console.WriteLine($"Deleted after:{r3.Time:yyyy:MM:dd HH:mm:ss.ffff}");
        }
        private static async Task RunCache(IServiceProvider provider)
        {
            ShowTitle("Run cache");
            var gt = provider.GetRequiredService<GetTime>();
            var finderFc = provider.GetRequiredService<AutoCacheService>();
            await finderFc.DeleteAsync<GetTime, DtObj>(x => x.NowTime1(1, 0));//Clear up
            var r1 = await gt.NowTime1(1, 0);
            Console.WriteLine($"First:{r1.Time:yyyy:MM:dd HH:mm:ss.ffff}");
            var r2 = await gt.NowTime1(1, 0);
            Console.WriteLine($"Second:{r1.Time:yyyy:MM:dd HH:mm:ss.ffff}");
            var delRes = await finderFc.DeleteAsync<GetTime, DtObj>(x => x.NowTime1(1, 0));
            Console.WriteLine($"Deleted Result:{delRes}");
            var r3 = await gt.NowTime1(1, 0);
            Console.WriteLine($"Deleted after:{r3.Time:yyyy:MM:dd HH:mm:ss.ffff}");
        }
        private static async Task RunCacheWithStatus(IServiceProvider provider)
        {
            ShowTitle("Run cache with status");
            var gt = provider.GetRequiredService<GetTime>();
            var finderFc = provider.GetRequiredService<AutoCacheService>();
            await finderFc.DeleteAsync<GetTime, DtObj>(x => x.NowTime(1, 0));//Clear up
            var r1 = await gt.NowTime(1, 0);
            Console.WriteLine($"First:{r1.RawData.Time:yyyy:MM:dd HH:mm:ss.ffff}, is {r1.Status}");
            var r2 = await gt.NowTime(1, 0);
            Console.WriteLine($"Second:{r1.RawData.Time:yyyy:MM:dd HH:mm:ss.ffff}, is {r2.Status}");
            var delRes = await finderFc.DeleteAsync<GetTime, DtObj>(x => x.NowTime(1, 0));
            Console.WriteLine($"Deleted Result:{delRes}");
            var r3 = await gt.NowTime(1, 0);
            Console.WriteLine($"Deleted after:{r3.RawData.Time:yyyy:MM:dd HH:mm:ss.ffff}, is {r3.Status}");
        }
        private static async Task RunLock(IServiceProvider provider)
        {
            ShowTitle("Run lock");
            var t = provider.GetRequiredService<LockTime>();
            var tsk = new Task[5];
            for (int i = 0; i < tsk.Length; i++)
            {
                tsk[i] = Task.Factory.StartNew(() =>
                {
                    t.Inc(10);
                });
            }
            var sw = Stopwatch.GetTimestamp();
            await Task.WhenAll(tsk);
            var ed = Stopwatch.GetTimestamp();
            Console.WriteLine($"并发加法{tsk.Length}次*10结果:"+t.A);
        }
    }
    public class LockTime
    {
        public virtual int A { get; set; }

        [AutoLock]
        public virtual void Inc([AutoLockSkipPart] int j)
        {
            for (int i = 0; i < j; i++)
            {
                A++;
            }
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
    public interface IGetTime
    {
        [AutoCache]
        [AutoCacheOptions(CanRenewal = false)]
        Task<AutoCacheResult<DtObj>> NowTime(int id, [AutoCacheSkipPart] double dd);

        [AutoCache]
        [AutoCacheOptions(CanRenewal = false)]
        Task<DtObj> NowTime1(int id, [AutoCacheSkipPart] long dd);
    }
    public class NoGetTime : IGetTime
    {
        public Task<AutoCacheResult<DtObj>> NowTime(int id, [AutoCacheSkipPart] double dd)
        {
            return Task.FromResult(new AutoCacheResult<DtObj> { RawData = new DtObj { Time = DateTime.Now } });
        }

        public Task<DtObj> NowTime1(int id, [AutoCacheSkipPart] long dd)
        {
            return Task.FromResult(new DtObj { Time = DateTime.Now });
        }
    }
    public class GetTime
    {
        [AutoCache]
        [AutoCacheOptions(CanRenewal = false)]
        public virtual Task<AutoCacheResult<DtObj>> NowTime( int id, [AutoCacheSkipPart] double dd)
        {
            //Console.WriteLine("yerp");
            return Task.FromResult(new AutoCacheResult<DtObj> { RawData = new DtObj { Time = DateTime.Now } });
        }
        [AutoCache]
        [AutoCacheOptions(CanRenewal = false)]
        public virtual Task<DtObj> NowTime1(int id, [AutoCacheSkipPart] long dd)
        {
            //Console.WriteLine("yerp");
            return Task.FromResult(new DtObj { Time = DateTime.Now });
        }
    }
    public class DtObj
    {
        public DateTime? Time { get; set; }
        public override string ToString()
        {
            return Time.ToString()!;
        }
    }
}

