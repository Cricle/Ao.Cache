using Ao.Cache.Redis.Annotations;
using System;

namespace Ao.Cache.WebApi
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }
        public int TemperatureC1 { get; set; }
        public int TemperatureC2 { get; set; }
        public int TemperatureC3 { get; set; }
        public int TemperatureC4 { get; set; }
        public int TemperatureC5 { get; set; }
        public int TemperatureC6 { get; set; }
        public int TemperatureC7 { get; set; }
        public int TemperatureC8 { get; set; }
        public int TemperatureC9 { get; set; }
        public int TemperatureC0 { get; set; }
        public int TemperatureC11 { get; set; }
        public int TemperatureC22 { get; set; }
        public int TemperatureC33 { get; set; }
        public int TemperatureC44 { get; set; }
        public int TemperatureC55 { get; set; }

        [IgnoreColumn]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}