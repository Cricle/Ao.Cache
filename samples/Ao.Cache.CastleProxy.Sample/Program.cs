using Microsoft.Extensions.DependencyInjection;
using DryIoc.Microsoft.DependencyInjection;
using System;
using Structing.DryInterceptor.Annotations;
using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.CastleProxy.Annotations;
using DryIoc;
using Ao.Cache.InMemory;
using Microsoft.Extensions.Caching.Memory;
using Ao.Cache.CastleProxy.Model;
using StackExchange.Redis;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
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
            //ser.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("127.0.0.1:6379"));
            //ser.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            //ser.AddSingleton<IDistributedLockFactory>(p =>
            //new RedLockFactory(new RedLockConfiguration(new ExistingMultiplexersRedLockConnectionProvider
            //{
            //    Multiplexers = new RedLockMultiplexer[]
            //      {
            //          new RedLockMultiplexer(p.GetRequiredService<IConnectionMultiplexer>())
            //      }
            //})));
            //ser.AddSingleton<ILockerFactory,InRedis.RedisLockFactory>();
            ser.AddSingleton<ILockerFactory, MemoryLockFactory>();
            ser.AddMemoryCache();
            ser.AddSingleton(typeof(IDataFinder<,>), typeof(DefaultInMemoryCacheFinder<,>));

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            icon.AsyncIntercept(typeof(GetTime), typeof(CacheInterceptor));
            icon.AsyncIntercept(typeof(LockTime), typeof(LockInterceptor));
            var provider = icon.BuildServiceProvider();

            RunCache(provider);
            //RunLock(provider);
        }
        private static void RunCache(IServiceProvider provider)
        {
            var gt = provider.GetRequiredService<GetTime>();
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(300));
                var sw = Stopwatch.GetTimestamp();
                var n = gt.NowTime(i % 3, i);
                var ed = Stopwatch.GetTimestamp();
                Console.WriteLine(new TimeSpan(ed-sw));
                //Console.WriteLine($"Data:{n.RawData!.Value:HH:mm:ss ffff}, Status:{n.Status}");
            }
        }
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
                        t.Inc(q*50);
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
        public virtual void Inc([AutoLockSkipPart]int j)
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
        public virtual AutoCacheResult<DateTime?> NowTime(int id,[AutoCacheSkipPart]long dd)
        {
            Console.WriteLine("yerp");
            return new AutoCacheResult<DateTime?> { RawData = DateTime.Now };
        }

        [AutoCache]
        public virtual DateTime? NowTime1(int id, long dd)
        {
            Console.WriteLine("yerp");
            return DateTime.Now;
        }
    }
}

