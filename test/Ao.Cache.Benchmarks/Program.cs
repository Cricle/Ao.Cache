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
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new MyConfig());
        }
    }
    public class MyConfig : ManualConfig
    {
        public MyConfig()
        {
            AddLogger(BenchmarkDotNet.Loggers.ConsoleLogger.Unicode);
            AddJob(Job.MediumRun.WithPlatform(Platform.X64).WithGcServer(true).WithRuntime(CoreRuntime.Core80));
            AddJob(Job.MediumRun.WithPlatform(Platform.X64).WithGcServer(true).WithRuntime(NativeAotRuntime.Net80).WithId("AOT"));
            AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
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