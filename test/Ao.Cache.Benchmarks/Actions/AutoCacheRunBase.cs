using Ao.Cache.Serizlier.TextJson;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    public abstract class AutoCacheRunBase
    {
        protected IServiceProvider provider;

        protected virtual void Regist(IServiceCollection services)
        {

        }

        protected virtual bool UseRedis()
        {
            return false;
        }
        protected void Run(int times, int concurrent, Action<int> action)
        {
            var ts = times / concurrent;
            for (int i = 0; i < concurrent; i++)
            {
                for (int j = 0; j < ts; j++)
                {
                     action(j % 5);
                }
            }
        }
        protected async Task Run(int times, int concurrent, Func<int, Task> action)
        {
            var tasks = new Task[concurrent];
            var ts = times / concurrent;
            for (int i = 0; i < concurrent; i++)
            {
                tasks[i] = await Task.Factory.StartNew(async () =>
                {
                    for (int j = 0; j < ts; j++)
                    {
                        await action(j % 5);
                    }
                });
            }
            await Task.WhenAll(tasks);
        }

        [GlobalSetup]
        public async Task Setup()
        {
            var ser = new ServiceCollection();
            ser.AddSingleton<GetTimeCt>();
            ser.AddSingleton<Gen.GetTimeCtProxy>();
            ser.AddSingleton<IDataAccesstor<int, Student>, AAccesstor>();
            Regist(ser);
            var s = ConfigurationOptions.Parse("127.0.0.1:6379");
            ser.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(s));
            ser.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            ser.AddSingleton<IEntityConvertor>(p =>
            {
                var opt = new JsonSerializerOptions(StudentJsonSerializerContext.Default.Options);
                return new TextJsonEntityConvertor(opt,Encoding.UTF8);
            });
            if (UseRedis())
            {
                ser.AddInRedisFinder();
            }
            else
            {
                ser.AddInMemoryFinder();
            }
            provider = ser.BuildServiceProvider();
            await OnSetup();
        }
        protected virtual Task OnSetup()
        {
            return Task.CompletedTask;
        }
    }
}
