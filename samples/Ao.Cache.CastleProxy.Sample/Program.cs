using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.CastleProxy.Model;
using Ao.Cache.InLitedb;
using Ao.Cache.InRedis;
using Ao.Cache.Serizlier.SpanJson;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using StackExchange.Redis.MultiplexerPool;
using System.Diagnostics;

namespace Ao.Cache.CastleProxy.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ThreadPool.SetMinThreads(100, 100);
            var ser = new ServiceCollection();
            ser.AddSingleton<GetTime>();
            ser.AddSingleton<LockTime>();
            ser.AddCastleCacheProxy();
            var pool = ConnectionMultiplexerPoolFactory.Create(
                poolSize: 1024,
                configuration: "127.0.0.1:6379",
                connectionSelectionStrategy: ConnectionSelectionStrategy.RoundRobin);
            ser.AddScoped<IConnectionMultiplexer>(x => pool.GetAsync().GetAwaiter().GetResult().Connection);
            //var s = ConfigurationOptions.Parse("127.0.0.1:6379");
            //s.SocketManager = new SocketManager(workerCount: 5, options: SocketManager.SocketManagerOptions.None); ;
            //ser.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(s));
            ser.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            ser.AddSingleton<IDistributedLockFactory>(p =>
            new RedLockFactory(new RedLockConfiguration(new ExistingMultiplexersRedLockConnectionProvider
            {
                Multiplexers = new RedLockMultiplexer[]
                  {
                      new RedLockMultiplexer(p.GetRequiredService<IConnectionMultiplexer>())
                  }
            })));
            ser.AddSingleton<ILockerFactory, RedisLockFactory>();
            ser.AddSingleton<IEntityConvertor, SpanJsonEntityConvertor>();
            ser.AddSingleton(typeof(IEntityConvertor<>), typeof(SpanJsonEntityConvertor<>));
            ser.AddSingleton(typeof(IDataFinderFactory), typeof(RedisDataFinderFactory));
            //var litedb = new LiteDatabase("a.db");
            //var d = litedb.GetCacheCollection();
            //d.EnsureIndex();
            //ser.AddSingleton<ILiteDatabase>(litedb);
            //ser.AddSingleton(d);
            //ser.AddSingleton(typeof(IDataFinderFactory), typeof(LitedbCacheFactory));

            //ser.AddSingleton<ILockerFactory, MemoryLockFactory>();
            //ser.AddSingleton(typeof(IDataFinderFactory<,>), typeof(InMemoryCacheFinderFactory<,>));
            //ser.AddMemoryCache();
            //ser.AddSingleton(typeof(IDataFinder<,>), typeof(DefaultInMemoryCacheFinder<,>));

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            icon.AsyncIntercept(typeof(GetTime), typeof(CacheInterceptor));
            icon.AsyncIntercept(typeof(LockTime), typeof(LockInterceptor));
            var provider = icon.BuildServiceProvider();
            var scope = provider.CreateScope();
            //var tsk = Stopwatch.GetTimestamp();
            //var task = new Task[5_000];
            //for (int i = 0; i < task.Length; i++)
            //{
            //    task[i] = Task.Factory.StartNew(Ad);
            //}
            //Task.WaitAll(task);
            //Console.WriteLine(new TimeSpan(Stopwatch.GetTimestamp() - tsk));
            //async Task Ad()
            //{
            //    using (var s = provider.CreateScope())
            //    {
            //        var db = s.ServiceProvider.GetRequiredService<IDatabase>();
            //        var readWrite = new List<Task>();
            //        for (int i = 0; i < 1_000; i++)
            //        {
            //            readWrite.Add(db.StringSetAsync("adsadsad", "dwdww"));
            //            readWrite.Add(db.StringGetAsync("adsadsad"));
            //        }
            //        await Task.WhenAll(readWrite);
            //    }
            //}
            RunCache(scope.ServiceProvider).GetAwaiter().GetResult();

            //RunLock(provider);
        }
        private static async Task RunCache(IServiceProvider provider)
        {
            var gt = provider.GetRequiredService<GetTime>();
            var finderFc = provider.GetRequiredService<AutoCacheService<DtObj>>();
            for (int q = 0; q < 3; q++)
            {
                //Thread.Sleep(TimeSpan.FromMilliseconds(300));
                var task = new Task[1];
                var sw = Stopwatch.GetTimestamp();
                for (int j = 0; j < task.Length; j++)
                {
                    task[j] =await Task.Factory.StartNew(async () =>
                    {
                        var n =await gt.NowTime1(q, 0);
                    });
                }
                await Task.WhenAll(task);
                var ed = Stopwatch.GetTimestamp();
                Console.WriteLine(new TimeSpan(ed - sw));
            }
            for (int q = 0; q < 3; q++)
            {
                //Thread.Sleep(TimeSpan.FromMilliseconds(300));
                var task = new Task[100000];
                var sw = Stopwatch.GetTimestamp();
                for (int j = 0; j < task.Length; j++)
                {
                    task[j] =await Task.Factory.StartNew(async () =>
                    {
                        var rd = new Task[1];
                        for (int q = 0; q < rd.Length; q++)
                        {
                            rd[q]= gt.NowTime1(q, 0);
                        }
                        await Task.WhenAll(rd);
                    });
                }
                await Task.WhenAll(task);
                var ed = Stopwatch.GetTimestamp();
                Console.WriteLine(new TimeSpan(ed - sw));
            }
        }
        private static int Rand(int a,int b) => a % b;
        private static int GetSP() => 3;
        private static void RunLock(IServiceProvider provider)
        {
            var t = provider.GetRequiredService<LockTime>();
            for (int j = 0; j < 10; j++)
            {
                t.A = 0;
                var tsk = new Task[20];
                for (int i = 0; i < tsk.Length; i++)
                {
                    var q = i;
                    tsk[i] = Task.Factory.StartNew(() =>
                    {
                        t.Inc(q * 50);
                    });
                }
                var sw = Stopwatch.GetTimestamp();
                Task.WaitAll(tsk);
                var ed = Stopwatch.GetTimestamp();
                Console.WriteLine(new TimeSpan(ed - sw));
                Console.WriteLine(t.A);
            }
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
        public virtual AutoCacheResult<DtObj> NowTime(int id, [AutoCacheSkipPart] double dd)
        {
            //Console.WriteLine("yerp");
            return new AutoCacheResult<DtObj> { RawData = new DtObj { Time= DateTime.Now } };
        }

        [AutoCache]
        [AutoCacheOptions(CanRenewal = false)]
        public virtual async Task<DtObj> NowTime1(int id, [AutoCacheSkipPart] long dd)
        {
            //Console.WriteLine("yerp");
            return new DtObj { Time = DateTime.Now };
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

