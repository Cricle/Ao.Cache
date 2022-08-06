using Ao.Cache.InRedis;
using Ao.Cache.InRedis.HashList.Finders;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ao.Cache.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherForecastDataFinder1 finder1;
        private readonly WeatherForecastDataFinder2 finder2;
        public WeatherForecastController(WeatherForecastDataFinder1 finder1,
            WeatherForecastDataFinder2 finder2)
        {
            this.finder2 = finder2;
            this.finder1 = finder1;
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
}