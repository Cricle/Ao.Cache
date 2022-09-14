using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.CastleProxy.Model;
using Ao.Cache.Events;
using Ao.Cache.Serizlier.TextJson;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Ao.Cache.CastleProxy.Sample
{
    class RedisAdapter : IEventAdapter
    {
        public RedisAdapter(IConnectionMultiplexer connectionMultiplexer, IEntityConvertor objectTransfer)
        {
            ConnectionMultiplexer = connectionMultiplexer;
            ObjectTransfer = objectTransfer;
            Subscriber = ConnectionMultiplexer.GetSubscriber();
        }

        public IConnectionMultiplexer ConnectionMultiplexer { get; }

        public ISubscriber Subscriber { get; }

        public IEntityConvertor ObjectTransfer { get; }

        public async Task<EventPublishResult> PublishAsync<T>(string channel, T data)
        {
            var res = await Subscriber.PublishAsync(channel, ObjectTransfer.ToBytes(data,typeof(T)));
            return new EventPublishResult(true);
        }

        public Task<IDisposable> SubscribeAsync<T>(string channel, IEventReceiver<T> receiver)
        {
            var res = new SubscribeToken<T>
            {
                Channel = channel,
                EventReceiver = receiver,
                ObjectTransfer = ObjectTransfer,
                Subscriber = Subscriber
            };
            res.Start();
            return Task.FromResult<IDisposable>(res);

        }
        class SubscribeToken<T>: SubscribeTokenBase
        {
            public IEventReceiver<T> EventReceiver;

            protected override Action<RedisChannel, RedisValue> CreateMethod()
            {
                return (_, v) =>
                {
                    EventReceiver.OnReceivedAsync(Channel, Case<T>(v)).ConfigureAwait(false);
                };
            }
        }
        abstract class SubscribeTokenBase:IDisposable
        {
            public Action<RedisChannel, RedisValue> Key;

            public RedisChannel Channel;

            public ISubscriber Subscriber;

            public IEntityConvertor ObjectTransfer;

            public void Start()
            {
                Key = CreateMethod();
                Subscriber.Subscribe(Channel, Key);
            }

            protected abstract Action<RedisChannel, RedisValue> CreateMethod();

            protected T Case<T>(in RedisValue value)
            {
                return (T)ObjectTransfer.ToEntry(value,typeof(T));
            }

            public void Dispose()
            {
                Subscriber.Unsubscribe(Channel,Key);
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var ser = new ServiceCollection();
            ser.AddSingleton<GetTime>();
            ser.AddSingleton<LockTime>();
            ser.AddSingleton<LockCache>();
            ser.AddSingleton<IEventAdapter, RedisAdapter>();
            ser.AddSingleton<IGetTime, NoGetTime>();
            ser.AddCastleCacheProxy();
            var redisCfg = Environment.GetEnvironmentVariable("USE_DOCKER_ENV") != null ?
                "redis:6379" : "127.0.0.1:6379";
            var s = ConfigurationOptions.Parse(redisCfg);
            ser.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(s));
            ser.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            ser.AddDistributedLockFactory();
            ser.AddSingleton<IEntityConvertor, TextJsonEntityConvertor>();
            ser.AddSingleton(typeof(IEntityConvertor<>), typeof(TextJsonEntityConvertor<>));
            //ser.AddInMemoryFinder();
            ser.AddInRedisFinder();

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            icon.AsyncIntercept<GetTime, CacheInterceptor>();
            icon.AsyncIntercept<LockCache, CacheInterceptor>();
            icon.AsyncIntercept<LockTime, LockInterceptor>();
            icon.AsyncIntercept<IGetTime, CacheInterceptor>();
            var provider = icon.BuildServiceProvider();
            provider.GetRequiredService<IEventAdapter>()
                .SubscribeAsync(EventHelper.GetChannelKey<DtObj>(null),
                new DelegateEventReceiver<AutoCacheEventPublishState<DtObj>>((_, a) =>
                {
                    Console.WriteLine($"是{a.Type}, 我是监听=======>" + a.Data.Time);
                    return Task.CompletedTask;
                }));
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
                tasks[i] = await Task.Factory.StartNew(async () =>
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
            await finderFc.DeleteAsync<GetTime, DtObj>(x => x.NowTime1(1));//Clear up
            var r1 = await gt.NowTime1(1);
            Console.WriteLine($"First:{r1.Time:yyyy:MM:dd HH:mm:ss.ffff}");
            var r2 = await gt.NowTime1(1);
            Console.WriteLine($"Second:{r1.Time:yyyy:MM:dd HH:mm:ss.ffff}");
            var delRes = await finderFc.DeleteAsync<GetTime, DtObj>(x => x.NowTime1(1));
            Console.WriteLine($"Deleted Result:{delRes}");
            var r3 = await gt.NowTime1(1);
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
            Console.WriteLine($"并发加法{tsk.Length}次*10结果:" + t.A);
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
        [AutoCacheOptions]
        [AutoCacheEvent(PublishType = AutoCacheEventPublishTypes.CacheFound | AutoCacheEventPublishTypes.MethodFound)]
        public virtual Task<AutoCacheResult<DtObj>> NowTime(int id, [AutoCacheSkipPart] double dd)
        {
            //Console.WriteLine("yerp");
            return Task.FromResult(new AutoCacheResult<DtObj> { RawData = new DtObj { Time = DateTime.Now } });
        }
        [AutoCache]
        public virtual Task<DtObj> NowTime1(int id)
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

