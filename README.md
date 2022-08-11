<h2 align="center">
Ao.Cache
</h2>
<h3 align="center">
A fast, easy, scalable cache and lock middleware
</h3>

<div>

</div>

# What is this

It is a fast, easy, scalable cache middleware, like that

```
request --> find cache ---miss---> find physical --> set in cache
                |
                L----> return
```

OR

```
request --> find cache ---miss---> find physical --> set in cache
                |
                L----> renewal cache time --> return
```

The library support auto do that, in `Ao.Cache.Core`

## IDataFinder<TIdentity, TEntity>

It support `One->One` cache mode, support `Delete`, `Exists`, `SetInCache`, `FindInCache`, `Renewal`

## IBatchDataFinder<TIdentity, TEntity>

It support `Many->Many` cache mode, support `Delete`, `Exists`, `SetInCache`, `FindInCache`, `Renewal` but all batch

## ILockerFactory

It support resource lock, in memory i realized it myself, in redis i use [RedLock.net](https://github.com/samcook/RedLock.net), not support in Litedb

## Lib - Ao.Cache.InRedis.HashList

This is a new mode of caching, use redis hash or list, it will cache object columns like that.

```csharp
public class WeatherForecast
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    [IgnoreColumn]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string Summary { get; set; }
}
```

When hash, it will cache

```cmd
hmset somekey Date dateValue TemperatureC TemperatureCValue Summary SummaryValue
```

It can modify some column in runtime, like `inc` or `dec` column without reset.

## Lib - Ao.Cache.CastleProxy

I use `Castle` to proxy method, it will very easy use cache, and very easy to lock method.

```csharp
public class GetTime
{
    [AutoCache]
    public virtual Task<AutoCacheResult<DtObj>> NowTime(int id, [AutoCacheSkipPart] double dd)
    {
        return Task.FromResult(new AutoCacheResult<DtObj> { RawData = new DtObj { Time = DateTime.Now } });
    }

    [AutoCache]
    [AutoCacheOptions(CanRenewal = false)]
    public virtual Task<DtObj> NowTime1(int id, [AutoCacheSkipPart] long dd)
    {
        return Task.FromResult(new DtObj { Time = DateTime.Now });
    }
}
public class LockTime
{
    public virtual int A { get; set; }

    [AutoLock]
    public virtual void Inc([AutoLockSkipPart] int j)
    {
        for (int i = 0; i < j; i++)
        {
            A++;
        }
    }
}

//I use dryioc and Structing.DryInterceptor to write this samples
//Install 

var ser = new ServiceCollection();
ser.AddSingleton<GetTime>();
ser.AddSingleton<LockTime>();       
//BEGIN:I will merge that in a new package later
ser.AddSingleton<ILockerFactory, MemoryLockFactory>();
ser.AddSingleton(typeof(IDataFinderFactory), typeof(InMemoryCacheFinderFactory));
ser.AddMemoryCache();
ser.AddSingleton(typeof(IDataFinder<,>), typeof(DefaultInMemoryCacheFinder<,>));
//END:I will merge that in a new package later

var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
    .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
icon.AsyncIntercept(typeof(GetTime), typeof(CacheInterceptor));
icon.AsyncIntercept(typeof(LockTime), typeof(LockInterceptor));
var provider = icon.BuildServiceProvider();
var scope = provider.CreateScope();
var gt = provider.GetRequiredService<GetTime>();
var res1=await gt.NowTime(q, 0);
var res2=await gt.NowTime(q, 0);
Assert.AreEquals(res1,res2);

var t = provider.GetRequiredService<LockTime>();

t.Inc(1);//It will lock when method executing
```

`[AutoCacheSkipPart]` will will indicate the composition of the cache key, default will use method full name and merge arguments withot tag `[AutoCacheSkipPart]`



# Why

## Why I use generic `TIdentity` not `string`

I think `string` can't reflect the identification of looking for cache, so i add one more step, but this is one-way.

## Why has interface `IDataAccesstor<TIdentity, TEntry>` and `IBatchDataAccesstor<TIdentity, TEntry>`

I think cache mode `can` split with physical data find and cache operator. So i only define how to fetch physical data.

## Why not define `IDataAccesstor<string,byte[]>`

I don't want to make `object<->byte[]` when not necessary, like in memory, sqlite etc.

## Why all method only async

I think cache always in other process, like redis, so i want all cache operator only async call. 

## Why has batch data finder

Sometimes there are such scenes, the request want find range data, but some exists, some miss, this is an embarrassing thing, so it will found miss in physical data, merge cache data and return.

# Nuget

|Package Id|version|
|:-:|:-:|
|Ao.Cache.Core|![](https://img.shields.io/nuget/dt/Ao.Cache.Core)|
|Ao.Cache.Redis|![](https://img.shields.io/nuget/dt/Ao.Cache.InMemory)|
|Ao.Cache.InRedis|![](https://img.shields.io/nuget/dt/Ao.Cache.InRedis)|
|Ao.Cache.InRedis.HashList|![](https://img.shields.io/nuget/dt/Ao.Cache.InRedis.HashList)|
|Ao.Cache.CastleProxy|![](https://img.shields.io/nuget/dt/Ao.Cache.CastleProxy)|
|Ao.Cache.Serizlier.MessagePack|![](https://img.shields.io/nuget/dt/Ao.Cache.Serizlier.MessagePack)|
|Ao.Cache.Serizlier.SpanJson|![](https://img.shields.io/nuget/dtAo.Cache.Serizlier.SpanJson)|
|Ao.Cache.Serizlier.TextJson|![](https://img.shields.io/nuget/dt/Ao.Cache.Serizlier.TextJson)|

# Next

- [ ] Fixed cache model
- [ ] Add tests
- [ ] Improve performance
