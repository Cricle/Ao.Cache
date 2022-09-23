using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.Serizlier.MessagePack;
using BenchmarkDotNet.Attributes;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Linq;
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
            ser.AddPooledDbContextFactory<StudentDbContext>(x =>
            {
                x.UseSqlite("Data Source=student.db;");
            }).AddDbContextPool<StudentDbContext>(x =>
            {
                x.UseSqlite("Data Source=student.db;");
            });
            ser.AddSingleton<GetTime>();
            ser.AddSingleton<GetTimeCt>();
            ser.AddSingleton<IDataAccesstor<int, Student>, AAccesstor>();
            ser.WithCastleCacheProxy();
            Regist(ser);
            var s = ConfigurationOptions.Parse("127.0.0.1:6379");
            ser.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(s));
            ser.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            ser.AddSingleton<IEntityConvertor, MessagePackEntityConvertor>();
            ser.AddSingleton(typeof(IEntityConvertor<>), typeof(MessagePackEntityConvertor<>));
            if (UseRedis())
            {
                ser.AddInRedisFinder();
            }
            else
            {
                ser.AddInMemoryFinder();
            }

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            icon.AsyncIntercept(typeof(GetTime), typeof(CacheInterceptor));
            provider = icon.BuildServiceProvider();
            MethodBoundaryAspect.Interceptors.GlobalMethodBoundary.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            //using (var scope = provider.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService<StudentDbContext>();
            //    db.Database.EnsureCreated();
            //    if (!db.Students.Any())
            //    {
            //        for (int i = 0; i < 1000; i++)
            //        {
            //            db.Students.Add(new Student
            //            {
            //                Name = i.ToString() + "dsadsad"
            //            });
            //        }
            //        db.SaveChanges();
            //    }
            //}
            await OnSetup();
        }
        protected virtual Task OnSetup()
        {
            return Task.CompletedTask;
        }
    }
}
