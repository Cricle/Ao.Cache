using BenchmarkDotNet.Running;

namespace Ao.Cache.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var r = new AutoCacheRun();
            //r.Times = 200_000;
            //r.Concurrent = 2000;
            //r.Setup().GetAwaiter().GetResult();
            //r.MethodBoundNoResult().GetAwaiter().GetResult();
            //Task.Delay(1000).GetAwaiter().GetResult();
            //r.MethodBoundNoResult().GetAwaiter().GetResult();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
        }
    }
}