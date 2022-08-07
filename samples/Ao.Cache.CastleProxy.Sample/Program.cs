using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.CastleProxy.Model;
using Ao.Cache.InMemory;
using Ao.Cache.InRedis;
using Ao.Cache.Serizlier.Apex;
using Ao.Cache.Serizlier.SpanJson;
using Ao.Cache.Serizlier.TextJson;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
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
            ThreadPool.SetMinThreads(10000,10000);
            var ser = new ServiceCollection();
            ser.AddSingleton<GetTime>();
            ser.AddSingleton<LockTime>();
            ser.AddCastleCacheProxy();
            //var pool = ConnectionMultiplexerPoolFactory.Create(
            //    poolSize: 1000,
            //    configuration: "127.0.0.1:6379",
            //    connectionSelectionStrategy: ConnectionSelectionStrategy.RoundRobin);
            //ser.AddScoped<IConnectionMultiplexer>(x => pool.GetAsync().GetAwaiter().GetResult().Connection);
            var s = ConfigurationOptions.Parse("127.0.0.1:6379");
            s.SocketManager= SocketManager.ThreadPool;
            s.PreserveAsyncOrder = false;
            ser.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(s));
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
            ser.AddSingleton(typeof(IDataFinderFactory<,>), typeof(RedisDataFinderFactory<,>));

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
                    task[j] = Task.Factory.StartNew(() =>
                    {
                        var n = gt.NowTime1(q, 0);
                    });
                }
                await Task.WhenAll(task);
                var ed = Stopwatch.GetTimestamp();
                Console.WriteLine(new TimeSpan(ed - sw));
            }
            for (int q = 0; q < 3; q++)
            {
                //Thread.Sleep(TimeSpan.FromMilliseconds(300));
                var task = new Task[500];
                var sw = Stopwatch.GetTimestamp();
                for (int j = 0; j < task.Length; j++)
                {
                    task[j] = Task.Factory.StartNew(() =>
                    {
                        for (int q = 0; q < 10; q++)
                        {
                            var n = gt.NowTime1(q, 0);
                        }
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
        public virtual DtObj NowTime1(int id, [AutoCacheSkipPart] long dd)
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
            return Time?.ToString();
        }
    }
}

