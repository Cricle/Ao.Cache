using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.CastleProxy.Model;
using Ao.Cache.Serizlier.MessagePack;
using BenchmarkDotNet.Attributes;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    [MemoryDiagnoser]
    public class AutoCacheRun
    {
        [Params(10_000, 20_000)]
        public int Times { get; set; }

        [Params(1000, 5000)]
        public int Concurrent { get; set; }

        IServiceProvider provider;

        [GlobalSetup]
        public void Setup()
        {
            var ser = new ServiceCollection();
            ser.AddPooledDbContextFactory<StudentDbContext>(x =>
            {
                x.UseSqlite("Data Source=student.db;");
            }).AddDbContextPool<StudentDbContext>(x =>
            {
                x.UseSqlite("Data Source=student.db;");
            });
            ser.AddScoped<GetTime>();
            ser.AddScoped<IDataAccesstor<int, Student>, AAccesstor>();
            ser.AddCastleCacheProxy();

            var s = ConfigurationOptions.Parse("127.0.0.1:6379");
            ser.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(s));
            ser.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            ser.AddDistributedLockFactory();
            ser.AddSingleton<IEntityConvertor, MessagePackEntityConvertor>();
            ser.AddSingleton(typeof(IEntityConvertor<>), typeof(MessagePackEntityConvertor<>));
            ser.AddInRedisFinder();

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            icon.AsyncIntercept(typeof(GetTime), typeof(CacheInterceptor));
            provider = icon.BuildServiceProvider();
            using (var scope = provider.CreateScope()) 
            {
                var db = scope.ServiceProvider.GetRequiredService<StudentDbContext>();
                db.Database.EnsureCreated();
                if (!db.Students.Any())
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        db.Students.Add(new Student
                        {
                            Name = i.ToString() + "dsadsad"
                        });
                    }
                    db.SaveChanges();
                }
            }
            var t = new Task[4];
            t[0] = Raw();
            t[1] = NoResult();
            t[2] = HasResult();
            t[3] = UseProvider();
            Task.WhenAll(t).GetAwaiter().GetResult();
            //NoResult().GetAwaiter().GetResult();
        }

        private async Task Run(Func<int,Task> action)
        {
            var tasks = new Task[Concurrent];
            var ts = Times / Concurrent;
            for (int i = 0; i < Concurrent; i++)
            {
                tasks[i] =await Task.Factory.StartNew(async () =>
                {
                    for (int j = 0; j < ts; j++)
                    {
                      await  action(j);
                    }
                });
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark(Baseline = true)]
        public async Task Raw()
        {
            await Run(async i =>
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
            await Run(async i =>
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
            await Run(async i =>
            {
                using (var scope = provider.CreateScope())
                {
                    var getTime = provider.GetService<GetTime>();
                   await getTime.NowTime(i % 5);
                }
            });
        }
        [Benchmark]
        public async Task UseProvider()
        {
            var tasks = new Task[Concurrent];
            var ts = Times / Concurrent;
            for (int i = 0; i < Concurrent; i++)
            {
                tasks[i] = await Task.Factory.StartNew(async () =>
                {
                    for (int j = 0; j < ts; j++)
                    {
                        using (var scope = provider.CreateScope())
                        {
                            var finder = scope.ServiceProvider.GetRequiredService<IDataFinder<int, Student>>();
                            await finder.FindAsync(i % 5);
                        }
                    }
                });
            }
            await Task.WhenAll(tasks);
        }
    }
    public class AAccesstor : IDataAccesstor<int, Student>
    {
        public AAccesstor(GetTime gt)
        {
            Gt = gt;
        }

        public GetTime Gt { get; set; }
        public Task<Student> FindAsync(int identity)
        {
            return Gt.Raw(identity);
        }
    }
    public class GetTime
    {
        private readonly IDbContextFactory<StudentDbContext> dbContextFactory;

        public GetTime(IDbContextFactory<StudentDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        [AutoCache]
        [AutoCacheOptions(CanRenewal = false)]
        public virtual async Task<AutoCacheResult<Student>> NowTime(int id)
        {
            return new AutoCacheResult<Student> { RawData =await Raw(id) };
        }

        [AutoCache]
        [AutoCacheOptions(CanRenewal =false)]
        public virtual async Task<Student> NowTime1(int id)
        {
           return await Raw(id);
        }

        public async Task<Student> Raw(int id)
        {
            using (var dbc = dbContextFactory.CreateDbContext())
            {
                return await dbc.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            }
        }
    }
}
