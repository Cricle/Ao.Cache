using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.CastleProxy.Model;
using Ao.Cache.InLitedb;
using Ao.Cache.InMemory;
using Ao.Cache.InRedis;
using Ao.Cache.Serizlier.SpanJson;
using Ao.Cache.Serizlier.TextJson;
using Ao.Cache.Serizlier.MessagePack;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Diagnostics;

namespace Ao.Cache.CastleProxy.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ser = new ServiceCollection();
            ser.AddSingleton<GetTime>();
            ser.AddSingleton<LockTime>();
            ser.AddCastleCacheProxy();
            var s = ConfigurationOptions.Parse("127.0.0.1:6379");
            ser.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(s));
            ser.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            ser.AddDistributedLockFactory();
            ser.AddSingleton<IEntityConvertor, MessagePackEntityConvertor>();
            ser.AddSingleton(typeof(IEntityConvertor<>),typeof(MessagePackEntityConvertor<>));
            ser.AddInRedisFinder();

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            icon.AsyncIntercept(typeof(GetTime), typeof(CacheInterceptor));
            icon.AsyncIntercept(typeof(LockTime), typeof(LockInterceptor));
            var provider = icon.BuildServiceProvider();
            var scope = provider.CreateScope();
            RunCache(scope.ServiceProvider).GetAwaiter().GetResult();
            RunCacheWithStatus(scope.ServiceProvider).GetAwaiter().GetResult();
            RunLock(scope.ServiceProvider).GetAwaiter().GetResult();
        }
        private static async Task RunCache(IServiceProvider provider)
        {
            Console.WriteLine("Run cache");
            var gt = provider.GetRequiredService<GetTime>();
            var finderFc = provider.GetRequiredService<AutoCacheService<DtObj>>();
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
            Console.WriteLine("Run cache with status");
            var gt = provider.GetRequiredService<GetTime>();
            var finderFc = provider.GetRequiredService<AutoCacheService<DtObj>>();
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
            Console.WriteLine("Run lock");
            var t = provider.GetRequiredService<LockTime>();
            var tsk = new Task[10];
            for (int i = 0; i < tsk.Length; i++)
            {
                tsk[i] = Task.Factory.StartNew(() =>
                {
                    t.Inc(1);
                });
            }
            var sw = Stopwatch.GetTimestamp();
            await Task.WhenAll(tsk);
            var ed = Stopwatch.GetTimestamp();
            Console.WriteLine(t.A);
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
    public class GetTime
    {
        [AutoCache]
        [AutoCacheOptions(CanRenewal = false)]
        public virtual Task<AutoCacheResult<DtObj>> NowTime(int id, [AutoCacheSkipPart] double dd)
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

