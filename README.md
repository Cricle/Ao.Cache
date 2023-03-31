<h2 align="center">
Ao.Cache
</h2>

<div align="center">

[![.NET Build](https://github.com/Cricle/Ao.Cache/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Cricle/Ao.Cache/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/Cricle/Ao.Cache/branch/main/graph/badge.svg?token=pVSwE02V1L)](https://codecov.io/gh/Cricle/Ao.Cache)

</div>

<h3 align="center">
A fast, easy, scalable cache and lock middleware(can compile time-dev)
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

> Detail can see in [Ao.Cache.Sample.CodeGen](./samples/Ao.Cache.Sample.CodeGen/)

The lib will auto-generate code in compile time, when use `Ao.Cache.CodeGen`(Support interface and class)

Like that

```csharp
//Will generate proxy for class Student
[CacheProxy(ProxyType =typeof(Student))]
public interface IStudent
{
    void Run();

    int? Get<T>(int? a);

    int Get1(A a);

    [CacheProxyMethod(CacheTime = "00:00:11")]
    Task<int> Get2(A a);

    ValueTask<int> Get3(A a);
}
```

## Run in hand code

It support call expression to action cache.(The method actual no execute)

```csharp
var creator = provider.GetRequiredService<ICacheHelperCreator>();
_ = finder.Get2(ax).Result;
Console.WriteLine(creator.DeleteAsync(() => finder.Get2(ax)).Result);//Delete by method expression
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
- [ ] Improve performance

# Test cover

![](https://codecov.io/gh/Cricle/Ao.Cache/branch/main/graphs/tree.svg?token=pVSwE02V1L)

# Benchmarks

The `v1.5.0-preview-1`

``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2006/22H2/2022Update)
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.202
  [Host]     : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2


```
|      Method | Times | Concurrent | IsUseRedis |       Mean |    Error |   StdDev | Ratio | RatioSD |     Gen0 |    Gen1 |   Gen2 |  Allocated | Alloc Ratio |
|------------ |------ |----------- |----------- |-----------:|---------:|---------:|------:|--------:|---------:|--------:|-------:|-----------:|------------:|
| **EasyCaching** |   **500** |         **10** |      **False** |   **324.1 μs** |  **4.24 μs** |  **3.54 μs** |  **1.00** |    **0.00** |  **51.7578** |       **-** |      **-** |  **423.41 KB** |        **1.00** |
| UseProvider |   500 |         10 |      False |   199.3 μs |  2.37 μs |  2.10 μs |  0.61 |    0.01 |  40.2832 |       - |      - |  329.62 KB |        0.78 |
|    UseProxy |   500 |         10 |      False |   183.7 μs |  2.59 μs |  2.43 μs |  0.57 |    0.01 |  28.3203 |       - |      - |  231.96 KB |        0.55 |
|             |       |            |            |            |          |          |       |         |          |         |        |            |             |
| **EasyCaching** |   **500** |         **10** |       **True** | **2,877.5 μs** | **25.85 μs** | **22.92 μs** |  **1.00** |    **0.00** |  **78.1250** | **23.4375** | **3.9063** |  **646.18 KB** |        **1.00** |
| UseProvider |   500 |         10 |       True | 3,068.0 μs | 41.07 μs | 36.41 μs |  1.07 |    0.01 |  82.0313 | 23.4375 | 3.9063 |   681.5 KB |        1.05 |
|    UseProxy |   500 |         10 |       True | 3,012.2 μs | 36.50 μs | 32.36 μs |  1.05 |    0.01 |  70.3125 | 23.4375 | 3.9063 |  579.86 KB |        0.90 |
|             |       |            |            |            |          |          |       |         |          |         |        |            |             |
| **EasyCaching** |   **500** |        **100** |      **False** |   **362.7 μs** |  **4.94 μs** |  **4.38 μs** |  **1.00** |    **0.00** |  **52.7344** |       **-** |      **-** |  **431.15 KB** |        **1.00** |
| UseProvider |   500 |        100 |      False |   249.5 μs |  3.41 μs |  3.19 μs |  0.69 |    0.01 |  41.0156 |       - |      - |  337.35 KB |        0.78 |
|    UseProxy |   500 |        100 |      False |   217.1 μs |  3.59 μs |  3.36 μs |  0.60 |    0.01 |  29.2969 |       - |      - |   239.7 KB |        0.56 |
|             |       |            |            |            |          |          |       |         |          |         |        |            |             |
| **EasyCaching** |   **500** |        **100** |       **True** |   **949.9 μs** | **19.36 μs** | **55.85 μs** |  **1.00** |    **0.00** |        **-** |       **-** |      **-** |  **664.65 KB** |        **1.00** |
| UseProvider |   500 |        100 |       True | 1,192.9 μs | 22.84 μs | 54.73 μs |  1.26 |    0.09 |  87.8906 | 23.4375 | 1.9531 |  697.62 KB |        1.05 |
|    UseProxy |   500 |        100 |       True | 1,178.7 μs | 23.55 μs | 45.94 μs |  1.24 |    0.09 |  76.1719 | 23.4375 | 1.9531 |  596.12 KB |        0.90 |
|             |       |            |            |            |          |          |       |         |          |         |        |            |             |
| **EasyCaching** |  **1000** |         **10** |      **False** |   **633.2 μs** |  **8.23 μs** |  **7.70 μs** |  **1.00** |    **0.00** | **102.5391** |       **-** |      **-** |  **845.33 KB** |        **1.00** |
| UseProvider |  1000 |         10 |      False |   382.5 μs |  5.65 μs |  5.01 μs |  0.61 |    0.01 |  80.5664 |       - |      - |  657.74 KB |        0.78 |
|    UseProxy |  1000 |         10 |      False |   357.8 μs |  0.85 μs |  0.75 μs |  0.57 |    0.01 |  56.6406 |       - |      - |  462.43 KB |        0.55 |
|             |       |            |            |            |          |          |       |         |          |         |        |            |             |
| **EasyCaching** |  **1000** |         **10** |       **True** | **5,663.1 μs** | **14.07 μs** | **13.16 μs** |  **1.00** |    **0.00** | **156.2500** | **46.8750** | **7.8125** | **1289.33 KB** |        **1.00** |
| UseProvider |  1000 |         10 |       True | 6,121.4 μs | 59.17 μs | 55.35 μs |  1.08 |    0.01 | 171.8750 | 39.0625 | 7.8125 | 1360.99 KB |        1.06 |
|    UseProxy |  1000 |         10 |       True | 5,942.9 μs | 38.90 μs | 32.49 μs |  1.05 |    0.01 | 140.6250 | 46.8750 | 7.8125 | 1156.93 KB |        0.90 |
|             |       |            |            |            |          |          |       |         |          |         |        |            |             |
| **EasyCaching** |  **1000** |        **100** |      **False** |   **633.1 μs** |  **3.00 μs** |  **2.66 μs** |  **1.00** |    **0.00** | **104.4922** |       **-** |      **-** |  **853.07 KB** |        **1.00** |
| UseProvider |  1000 |        100 |      False |   463.2 μs |  2.14 μs |  1.90 μs |  0.73 |    0.01 |  81.5430 |  0.4883 |      - |  665.48 KB |        0.78 |
|    UseProxy |  1000 |        100 |      False |   384.2 μs |  3.63 μs |  3.22 μs |  0.61 |    0.00 |  57.6172 |       - |      - |  470.17 KB |        0.55 |
|             |       |            |            |            |          |          |       |         |          |         |        |            |             |
| **EasyCaching** |  **1000** |        **100** |       **True** | **1,678.6 μs** | **33.41 μs** | **94.77 μs** |  **1.00** |    **0.00** |        **-** |       **-** |      **-** | **1302.24 KB** |        **1.00** |
| UseProvider |  1000 |        100 |       True | 2,127.1 μs | 42.18 μs | 78.19 μs |  1.28 |    0.09 | 171.8750 | 46.8750 | 3.9063 | 1371.99 KB |        1.05 |
|    UseProxy |  1000 |        100 |       True | 2,156.8 μs | 41.62 μs | 55.57 μs |  1.30 |    0.07 | 148.4375 | 46.8750 | 3.9063 | 1169.49 KB |        0.90 |
