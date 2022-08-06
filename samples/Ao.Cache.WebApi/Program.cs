using Ao.Cache.InRedis.HashList.Finders;
using Ao.Cache.WebApi.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(x => x.Configuration = "127.0.0.1:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("127.0.0.1:6379"));
builder.Services.AddScoped<IDatabase>(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

builder.Services.AddScoped<WeatherForecastDataFinder1>();
builder.Services.AddScoped<WeatherForecastDataFinder2>();
builder.Services.AddScoped<CacheFinderManager>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
