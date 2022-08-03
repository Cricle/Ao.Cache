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

namespace Ao.Cache.Benchmarks.Actions
{
    [MemoryDiagnoser]
    public class AutoCacheRun
    {
        [Params(10, 1000, 50000)]
        public int Times { get; set; }

        GetTime getTime;
        [GlobalSetup]
        public void Setup()
        {
            var ser = new ServiceCollection();
            ser.AddSingleton<GetTime>();
            ser.AddCastleCacheProxy();
            ser.AddSingleton<ILockerFactory, MemoryLockFactory>();
            ser.AddMemoryCache();
            ser.AddSingleton(typeof(IDataFinder<,>), typeof(DefaultInMemoryCacheFinder<,>));

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            icon.AsyncIntercept(typeof(GetTime), typeof(CacheInterceptor));
            var provider = icon.BuildServiceProvider();
            getTime = provider.GetService<GetTime>();
            Raw();
            NowTime1();
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
        public void NowTime1()
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
    }


    public class GetTime
    {
        [AutoCache]
        public virtual AutoCacheResult<DateTime?> NowTime(int id, [AutoCacheSkipPart] long dd)
        {
            return new AutoCacheResult<DateTime?> { RawData = DateTime.Now };
        }

        [AutoCache]
        public virtual DateTime? NowTime1(int id, [AutoCacheSkipPart] long dd)
        {
            return DateTime.Now;
        }
        public DateTime? Raw(int id, long dd)
        {
            return DateTime.Now;
        }
    }
}
