using BenchmarkDotNet.Attributes;
using EasyCaching.Core;
using EasyCaching.Serialization.SystemTextJson.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    [MemoryDiagnoser]
    public class AutoCacheVsEasyCachingSync : AutoCacheRunBase
    {
        [Params(1000)]
        public int Times { get; set; }

        [Params(100)]
        public int Concurrent { get; set; }

        [Params(true, false)]
        public bool IsUseRedis { get; set; }

        protected override bool UseRedis()
        {
            return IsUseRedis;
        }
        protected override void Regist(IServiceCollection services)
        {
            base.Regist(services);
            services.AddEasyCaching(x =>
            {
                if (UseRedis())
                {
                    x.UseRedis(config =>
                    {
                        config.DBConfig.Endpoints.Add(new EasyCaching.Core.Configurations.ServerEndPoint("127.0.0.1", 6379));
                        config.DBConfig.SyncTimeout = 10000;
                        config.DBConfig.AsyncTimeout = 10000;
                        config.SerializerName = "mymsgpack";
                    }, "m1").WithSystemTextJson(x =>
                    {
                        x.AddContext<StudentJsonSerializerContext>();
                    }, "mymsgpack");
                }
                else
                {

                    x.UseInMemory("m1");
                }

            });
        }

        [Benchmark(Baseline = true)]
        public void EasyCaching()
        {
            Run(Times, Concurrent, i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var factory = scope.ServiceProvider.GetService<IEasyCachingProviderFactory>();
                    var mCache = factory.GetCachingProvider("m1");
                    var q = i % 5;
                    var str = "Mo.Cache.Benchmarks.Actions.GetTime.Now." + q;
                    var stu = mCache.Get<Student>(str);
                    if (!stu.HasValue)
                    {
                        var s = scope.ServiceProvider.GetRequiredService<GetTimeCt>().RawSync(i, null, i);
                        mCache.Set(str, s, TimeSpan.FromSeconds(3));
                    }
                }
            });
        }
        public void UseProvider1()
        {
            for (int i = 0; i < Times * Concurrent; i++)
            {
                using (var scope = provider.CreateScope())
                {
                    var finder = scope.ServiceProvider.GetRequiredService<ISyncDataFinder<int, Student>>();
                    var q = i % 5;
                    var cache = finder.FindInCache(q);
                    if (cache == null)
                    {
                        var s = scope.ServiceProvider.GetRequiredService<GetTimeCt>().RawSync(i,null,i);
                        finder.SetInCache(q, s);
                    }
                }

            }

        }
        [Benchmark]
        public void UseProvider()
        {
            Run(Times, Concurrent, i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var finder = scope.ServiceProvider.GetRequiredService<ISyncDataFinder<int, Student>>();
                    var q = i % 5;
                    var cache = finder.FindInCache(q);
                    if (cache == null)
                    {
                        var s = scope.ServiceProvider.GetRequiredService<GetTimeCt>().RawSync(i, null, i);
                        finder.SetInCache(q, s);
                    }
                }
            });

        }
        [Benchmark]
        public void UseProxy()
        {
            Run(Times, Concurrent, i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var finder = scope.ServiceProvider.GetRequiredService<Gen.GetTimeCtProxy>();
                    finder.NowTimeSync(i % 5,null,i);
                }
            });

        }
    }

    [MemoryDiagnoser]
    public class AutoCacheVsEasyCaching : AutoCacheRunBase
    {
        [Params(500, 1000)]
        public int Times { get; set; }

        [Params(10, 100)]
        public int Concurrent { get; set; }

        [Params(true, false)]
        public bool IsUseRedis { get; set; }

        protected override bool UseRedis()
        {
            return IsUseRedis;
        }
        protected override void Regist(IServiceCollection services)
        {
            base.Regist(services);
            services.AddEasyCaching(x =>
            {
                if (UseRedis())
                {
                    x.UseRedis(config =>
                    {
                        config.DBConfig.Endpoints.Add(new EasyCaching.Core.Configurations.ServerEndPoint("127.0.0.1", 6379));
                        config.DBConfig.SyncTimeout = 10000;
                        config.DBConfig.AsyncTimeout = 10000;
                        config.SerializerName = "mymsgpack";
                    }, "m1").WithSystemTextJson(x =>
                    {
                        x.AddContext<StudentJsonSerializerContext>();
                    }, "mymsgpack");
                }
                else
                {

                    x.UseInMemory("m1");
                }

            });
        }

        [Benchmark(Baseline = true)]
        public async Task EasyCaching()
        {
            await Run(Times, Concurrent, async i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var factory = scope.ServiceProvider.GetService<IEasyCachingProviderFactory>();
                    var mCache = factory.GetCachingProvider("m1");
                    var q = i % 5;
                    var str = "Mo.Cache.Benchmarks.Actions.GetTime.Now." + q;
                    var stu = await mCache.GetAsync<Student>(str);
                    if (!stu.HasValue)
                    {
                        var s = await scope.ServiceProvider.GetRequiredService<GetTimeCt>().Raw(i, null, i);
                        await mCache.SetAsync(str, s, TimeSpan.FromSeconds(3));
                    }
                }
            });
        }
        public async Task UseProvider1()
        {
            for (int i = 0; i < Times*Concurrent; i++)
            {
                using (var scope = provider.CreateScope())
                {
                    var finder = scope.ServiceProvider.GetRequiredService<IDataFinder<int, Student>>();
                    var q = i % 5;
                    var cache = await finder.FindInCacheAsync(q);
                    if (cache == null)
                    {
                        var s = await scope.ServiceProvider.GetRequiredService<GetTimeCt>().Raw(i, null, i);
                        await finder.SetInCacheAsync(q, s);
                    }
                }

            }

        }
        [Benchmark]
        public async Task UseProvider()
        {
            await Run(Times, Concurrent, async i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var finder = scope.ServiceProvider.GetRequiredService<IDataFinder<int, Student>>();
                    var q = i % 5;
                    var cache =await finder.FindInCacheAsync(q);
                    if (cache == null)
                    {
                        var s = await scope.ServiceProvider.GetRequiredService<GetTimeCt>().Raw(i, null, i);
                        await finder.SetInCacheAsync(q, s);
                    }
                }
            });

        }
        [Benchmark]
        public async Task UseProxy()
        {
            await Run(Times, Concurrent, async i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var finder = scope.ServiceProvider.GetRequiredService<Gen.GetTimeCtProxy>();
                    await finder.NowTime(i % 5, null, i);
                }
            });

        }
    }
}
