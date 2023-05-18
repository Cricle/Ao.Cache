using Ao.Cache.InMemory;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace Ao.Cache.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            _ = typeof(InMemoryCacheFinder<int,Student>);
            //var r = new AutoCacheVsEasyCaching();
            //r.Times = 100;
            //r.Concurrent = 100;
            //r.IsUseRedis = true;
            //r.Setup().GetAwaiter().GetResult();
            //r.UseProxy().GetAwaiter().GetResult();
            //r.UseProvider().GetAwaiter().GetResult();
            //r.EasyCaching().GetAwaiter().GetResult();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new MyConfig());
        }
    }
    public class MyConfig : ManualConfig
    {
        public MyConfig()
        {
            AddLogger(BenchmarkDotNet.Loggers.ConsoleLogger.Unicode);
            AddJob(Job.ShortRun.WithPlatform(Platform.X64).WithGcServer(true).WithRuntime(CoreRuntime.Core70));
            AddJob(Job.ShortRun.WithPlatform(Platform.X64).WithGcServer(true).WithRuntime(NativeAotRuntime.Net70).WithStrategy(BenchmarkDotNet.Engines.RunStrategy.ColdStart).WithId("AOT"));
            AddExporter(BenchmarkDotNet.Exporters.DefaultExporters.Markdown);
            AddExporter(BenchmarkDotNet.Exporters.DefaultExporters.Csv);
            AddAnalyser(EnvironmentAnalyser.Default
                , OutliersAnalyser.Default
                , MinIterationTimeAnalyser.Default
                , MultimodalDistributionAnalyzer.Default
                , RuntimeErrorAnalyser.Default
                , ZeroMeasurementAnalyser.Default
                , BaselineCustomAnalyzer.Default
                , HideColumnsAnalyser.Default
                );
            AddValidator(BaselineValidator.FailOnError
                , SetupCleanupValidator.FailOnError
                , JitOptimizationsValidator.FailOnError
                , RunModeValidator.FailOnError
                , GenericBenchmarksValidator.DontFailOnError
                , DeferredExecutionValidator.FailOnError
                , ParamsAllValuesValidator.FailOnError
                );
            AddColumnProvider(BenchmarkDotNet.Columns.DefaultColumnProviders.Instance);
        }
    }
}