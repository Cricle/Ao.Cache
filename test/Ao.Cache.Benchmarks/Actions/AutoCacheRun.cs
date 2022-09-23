using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    [MemoryDiagnoser]
    public class AutoCacheRun : AutoCacheRunBase
    {
        [Params(10_000, 20_000)]
        public int Times { get; set; }

        [Params(1000, 5000)]
        public int Concurrent { get; set; }

        protected override async Task OnSetup()
        {
            await Raw();
            await NoResult();
            await HasResult();
            await MethodBoundNoResult();
            await MethodBoundResult();
            await UseProvider();
        }

        [Benchmark(Baseline = true)]
        public async Task Raw()
        {
            await Run(Times, Concurrent, async i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var getTime = provider.GetService<GetTime>();
                    await getTime.Raw(i % 5);
                }
            });
        }
        [Benchmark]
        public async Task NoResult()
        {
            await Run(Times, Concurrent, async i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var getTime = provider.GetService<GetTime>();
                    await getTime.NowTime1(i % 5);
                }
            });
        }
        [Benchmark]
        public async Task HasResult()
        {
            await Run(Times, Concurrent, async i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var getTime = provider.GetService<GetTime>();
                    await getTime.NowTime(i % 5);
                }
            });
        }
        [Benchmark]
        public async Task MethodBoundNoResult()
        {
            await Run(Times, Concurrent, async i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var getTime = provider.GetService<GetTimeCt>();
                    await getTime.NowTime(i % 5);
                }
            });
        }
        [Benchmark]
        public async Task MethodBoundResult()
        {
            await Run(Times, Concurrent, async i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var getTime = provider.GetService<GetTimeCt>();
                    await getTime.NowTime1(i % 5);
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
