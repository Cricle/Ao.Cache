<h2 align="center">
Ao.Cache
</h2>

<div align="center">

[![.NET Build](https://github.com/Cricle/Ao.Cache/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Cricle/Ao.Cache/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/Cricle/Ao.Cache/branch/main/graph/badge.svg?token=pVSwE02V1L)](https://codecov.io/gh/Cricle/Ao.Cache)

</div>

<h3 align="center">
A fast, easy, scalable cache, raw performace, cache proxy and lock middleware!
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

## Proxy

> Detail can see in [Ao.Cache.Sample.CodeGen](./samples/Ao.Cache.Sample.CodeGen/), it can be use in .NET7 AOT mode

The lib will auto-generate code in compile time, when use `Ao.Cache.CodeGen`(Support interface and class)

Like that(It support **async** or **sync** method, also support **ValueType**)

```csharp
//Will generate proxy for class Student default for 
[CacheProxy(ProxyType =typeof(Student)),Head="a.b.c"]//Head is cache key prefx
public interface IStudent
{
    void Run();//Will not proxy, return is void

    int? Get<T>(int? a);//Support for generic methods

    int Get1(A a);//Will be warinning

    [CacheProxyMethod(CacheTime = "00:00:11")]
    Task<int?> Get2(A a);//Can async proxy

    ValueTask<int?> Get3(A a);//Can valuetask async proxy
}
```

## Run in hand code

It support call expression to action cache.(The method actual no executed)

```csharp
var creator = provider.GetRequiredService<ICacheHelperCreator>();
_ = finder.Get2(ax).Result;
Console.WriteLine(creator.DeleteAsync(() => finder.Get2(ax)).Result);//Delete by method expression but no executed.
```

# Nuget

|Package Id|version|
|:-:|:-:|
|Ao.Cache.Core|![](https://img.shields.io/nuget/dt/Ao.Cache.Core)|
|Ao.Cache.Redis|![](https://img.shields.io/nuget/dt/Ao.Cache.InMemory)|
|Ao.Cache.InRedis|![](https://img.shields.io/nuget/dt/Ao.Cache.InRedis)|
|Ao.Cache.Serizlier.MessagePack|![](https://img.shields.io/nuget/dt/Ao.Cache.Serizlier.MessagePack)|
|Ao.Cache.Serizlier.SpanJson|![](https://img.shields.io/nuget/dt/Ao.Cache.Serizlier.SpanJson)|
|Ao.Cache.Serizlier.TextJson|![](https://img.shields.io/nuget/dt/Ao.Cache.Serizlier.TextJson)|
|Ao.Cache.CodeGen|![](https://img.shields.io/nuget/dt/Ao.Cache.CodeGen)|

# Next

- [ ] Add tests
- [ ] Can AOT compile by no proxy
- [ ] Completely non-invasive code
- [ ] Improve performance

# Benchmarks

The `v1.5.0`

[benchmark](./test/Benchmarks.md)