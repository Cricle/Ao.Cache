using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.CastleProxy.Model;
using Ao.Cache.InMemory;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using Ao.Cache.CastleProxy;

namespace Ao.Cache.Benchmarks.Actions
{
    [MemoryDiagnoser]
    public class AutoCacheRun
    {
        [Params(10, 1000, 50000)]
        public int Times { get; set; }

        IServiceProvider provider;

        GetTime getTime;
        [GlobalSetup]
        public void Setup()
        {
            var ser = new ServiceCollection();
            ser.AddSingleton<GetTime>();
            ser.AddSingleton<IDataAccesstor<int, int>,AAccesstor>();
            ser.AddCastleCacheProxy();
            ser.AddSingleton<ILockerFactory, MemoryLockFactory>();
            ser.AddMemoryCache();
            ser.AddSingleton(typeof(IDataFinderFactory<,>), typeof(InMemoryCacheFinderFactory<,>));
            ser.AddSingleton(typeof(IDataFinder<,>), typeof(DefaultInMemoryCacheFinder<,>));

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            icon.AsyncIntercept(typeof(GetTime), typeof(CacheInterceptor));
            provider = icon.BuildServiceProvider();
            getTime = provider.GetService<GetTime>();
            Raw();
            NoResult();
            HasResult();
        }
        [Benchmark(Baseline = true)]
        public void Raw()
        {
            for (int i = 0; i < Times; i++)
            {
                getTime.Raw(i % 5, i);
            }
        }
        [Benchmark]
        public void NoResult()
        {
            for (int i = 0; i < Times; i++)
            {
                getTime.NowTime1(i % 5, i);
            }
        }
        [Benchmark]
        public void HasResult()
        {
            for (int i = 0; i < Times; i++)
            {
                getTime.NowTime(i % 5, i);
            }
        }
        [Benchmark]
        public async Task UseProvider()
        {
            for (int i = 0; i < Times; i++)
            {
                using (var scope = provider.CreateScope())
                {
                    var finder = scope.ServiceProvider.GetRequiredService<IDataFinder<int, int>>();
                    await finder.FindAsync(i % 5);
                }
            }
        }
    }
    public class AAccesstor : IDataAccesstor<int, int>
    {
        public AAccesstor(GetTime gt)
        {
            Gt = gt;
        }

        public GetTime Gt { get; set; }
        public Task<int> FindAsync(int identity)
        {
            return Task.FromResult(Gt.Raw(identity,0));
        }
    }
    public class GetTime
    {
        [AutoCache]
        public virtual AutoCacheResult<int> NowTime(int id, [AutoCacheSkipPart] long dd)
        {
            return new AutoCacheResult<int> { RawData = Raw(id, dd)};
        }

        [AutoCache]
        public virtual int NowTime1(int id, [AutoCacheSkipPart] long dd)
        {
            return Raw(id, dd);   
        }

        public int Raw(int id, long dd)
        {
            var lst = new List<int>();
            for (int j = 0; j < id; j++)
            {
                for (int q = 0; q < id; q++)
                {
                    lst.Add(j + q);
                }
            }
            if (lst.Count / 2 == 0)
            {
                return lst.Sum(x => x);
            }
            lst.Sort();
            return lst.FirstOrDefault();
        }
    }
}
