using Ao.Cache.MessagePack.Redis;
using Ao.Cache.Redis.Finders;
using Ao.Cache.TextJson.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ao.Cache.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherForecastDataFinder finder;
        private readonly WeatherForecastDataFinder1 finder1;
        private readonly WeatherForecastDataFinder2 finder2;
        private readonly WeatherForecastDataFinder3 finder3;
        public WeatherForecastController(WeatherForecastDataFinder finder,
            WeatherForecastDataFinder1 finder1,
            WeatherForecastDataFinder2 finder2,
            WeatherForecastDataFinder3 finder3)
        {
            this.finder3 = finder3;
            this.finder2=finder2;
            this.finder1 = finder1;
            this.finder = finder;
        }

        [HttpGet("[action]/{name}")]
        public async Task<IActionResult> Get(string name)
        {
            var s = Stopwatch.GetTimestamp();
            for (int i = 0; i < 1; i++)
            {
                var d = await finder.FindAsync(name);
            }
            return Ok(new TimeSpan(Stopwatch.GetTimestamp()-s));
        }
        [HttpGet("[action]/{name}")]
        public async Task<IActionResult> GetJson(string name)
        {
            var s = Stopwatch.GetTimestamp();
            for (int i = 0; i < 1; i++)
            {
                var d = await finder3.FindAsync(name);
            }
            return Ok(new TimeSpan(Stopwatch.GetTimestamp() - s));
        }
        [HttpGet("[action]/{name}")]
        public async Task<IActionResult> GetList(string name)
        {
            var s = Stopwatch.GetTimestamp();
            for (int i = 0; i < 1; i++)
            {
                var d = await finder1.FindAsync(name);
            }
            return Ok(new TimeSpan(Stopwatch.GetTimestamp() - s));
        }
        [HttpGet("[action]/{name}")]
        public async Task<IActionResult> GetHash(string name)
        {
            var s = Stopwatch.GetTimestamp();
            for (int i = 0; i < 1; i++)
            {
                var d = await finder2.FindAsync(name);
            }
            return Ok(new TimeSpan(Stopwatch.GetTimestamp() - s));
        }
    }
    public class WeatherForecastDataFinder2 : HashCacheFinder<string, WeatherForecast>
    {
        //private readonly IMemoryCache memoryCache;
        private readonly IDatabase database;

        public WeatherForecastDataFinder2(IDatabase database)
        {
            this.database = database;
            Build();
        }

        public override IDatabase GetDatabase()
        {
            return database;
        }

        protected override Task<WeatherForecast> OnFindInDbAsync(string identity)
        {
            return Task.FromResult(new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = identity
            });
        }
    }
    public class WeatherForecastDataFinder1 : ListCacheFinder<string, WeatherForecast>
    {
        //private readonly IMemoryCache memoryCache;
        private readonly IDatabase database;
        public WeatherForecastDataFinder1(IDatabase database)
        {
            this.database = database;
            Build();
        }

        public override IDatabase GetDatabase()
        {
            return database;
        }

        protected override Task<WeatherForecast> OnFindInDbAsync(string identity)
        {
            return Task.FromResult(new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = identity
            });
        }
    }
    [JsonSerializable(typeof(WeatherForecast))]
    internal partial class WeatherForecastContext : JsonSerializerContext
    {
    }
    public class WeatherForecastDataFinder3 : RedisJsonDataFinder<string, WeatherForecast>
    {
        public WeatherForecastDataFinder3(IDatabase database) : base(database)
        {
        }
        protected override Task<WeatherForecast> OnFindInDbAsync(string identity)
        {
            return Task.FromResult(new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = identity
            });
        }
    }
    public class WeatherForecastDataFinder : RedisMessagePackDataFinder<string, WeatherForecast>
    {
        public WeatherForecastDataFinder(IDatabase database) : base(database)
        {
        }

        protected override Task<WeatherForecast> OnFindInDbAsync(string identity)
        {
            return Task.FromResult(new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = identity
            });
        }
    }
}