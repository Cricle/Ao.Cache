using Ao.Cache.Benchmarks.Actions;
using BenchmarkDotNet.Running;

namespace Ao.Cache.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var r = new AutoCacheRun();
            //r.Times = 1;
            //r.Concurrent = 1;
            //r.Setup();

            //r.HasResult();
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
        }
    }
}