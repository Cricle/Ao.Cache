using BenchmarkDotNet.Attributes;
using EasyCaching.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    [MemoryDiagnoser]
    public class AutoCacheVsEasyCaching:AutoCacheRunBase
    {
        [Params(500, 1000)]
        public int Times { get; set; }

        [Params(10,100)]
        public int Concurrent { get; set; }

        [Params(true,false)]
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
                    }, "m1").WithMessagePack("mymsgpack");
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
                        var s = await scope.ServiceProvider.GetRequiredService<GetTime>().Raw(i);
                        mCache.Set(str, s, TimeSpan.FromSeconds(3));
                    }
                }
            });
        }
        [Benchmark]
        public async Task UseProvider()
        {
            await Run(Times, Concurrent, async i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var finder = scope.ServiceProvider.GetRequiredService<IDataFinder<int, Student>>();
                    if (finder.Options is DefaultDataFinderOptions<int, Student> op)
                    {
                        op.IsCanRenewal = false;
                    }
                    await finder.FindAsync(i % 5);
                }
            });

        }
    }
}
