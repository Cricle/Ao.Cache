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

The `v1.5.0-preview-2`

``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22000.1696/21H2/SunValley)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.202
  [Host]     : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2


```
|      Method | Times | Concurrent | IsUseRedis |        Mean |     Error |    StdDev | Ratio | RatioSD |     Gen0 |    Gen1 |    Gen2 |  Allocated | Alloc Ratio |
|------------ |------ |----------- |----------- |------------:|----------:|----------:|------:|--------:|---------:|--------:|--------:|-----------:|------------:|
| **EasyCaching** |   **500** |         **10** |      **False** |    **370.7 μs** |   **1.28 μs** |   **1.07 μs** |  **1.00** |    **0.00** |  **51.7578** |       **-** |       **-** |  **423.41 KB** |        **1.00** |
| UseProvider |   500 |         10 |      False |    210.7 μs |   1.11 μs |   1.04 μs |  0.57 |    0.00 |  34.4238 |       - |       - |  282.74 KB |        0.67 |
|    UseProxy |   500 |         10 |      False |    225.4 μs |   0.93 μs |   0.87 μs |  0.61 |    0.00 |  28.3203 |       - |       - |  231.96 KB |        0.55 |
|             |       |            |            |             |           |           |       |         |          |         |         |            |             |
| **EasyCaching** |   **500** |         **10** |       **True** |  **5,990.8 μs** |  **60.38 μs** |  **56.48 μs** |  **1.00** |    **0.00** |  **78.1250** | **23.4375** |  **7.8125** |  **645.66 KB** |        **1.00** |
| UseProvider |   500 |         10 |       True |  5,969.1 μs |  33.94 μs |  28.34 μs |  1.00 |    0.01 |  70.3125 | 23.4375 |  7.8125 |  614.26 KB |        0.95 |
|    UseProxy |   500 |         10 |       True |  5,957.9 μs | 119.02 μs | 122.22 μs |  1.00 |    0.02 |  70.3125 | 23.4375 |  7.8125 |  579.07 KB |        0.90 |
|             |       |            |            |             |           |           |       |         |          |         |         |            |             |
| **EasyCaching** |   **500** |        **100** |      **False** |    **407.5 μs** |   **1.98 μs** |   **1.85 μs** |  **1.00** |    **0.00** |  **52.7344** |       **-** |       **-** |  **431.15 KB** |        **1.00** |
| UseProvider |   500 |        100 |      False |    258.3 μs |   4.18 μs |   3.91 μs |  0.63 |    0.01 |  35.6445 |       - |       - |  290.48 KB |        0.67 |
|    UseProxy |   500 |        100 |      False |    249.3 μs |   1.96 μs |   1.74 μs |  0.61 |    0.01 |  29.2969 |       - |       - |   239.7 KB |        0.56 |
|             |       |            |            |             |           |           |       |         |          |         |         |            |             |
| **EasyCaching** |   **500** |        **100** |       **True** |  **1,403.7 μs** |  **46.65 μs** | **136.08 μs** |  **1.00** |    **0.00** |        **-** |       **-** |       **-** |   **662.4 KB** |        **1.00** |
| UseProvider |   500 |        100 |       True |  1,758.0 μs |  40.99 μs | 119.57 μs |  1.27 |    0.16 |  80.0781 | 21.4844 |  1.9531 |  630.08 KB |        0.95 |
|    UseProxy |   500 |        100 |       True |  1,859.6 μs |  36.72 μs |  75.01 μs |  1.31 |    0.13 |  74.2188 | 21.4844 |  1.9531 |  594.98 KB |        0.90 |
|             |       |            |            |             |           |           |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |         **10** |      **False** |    **807.4 μs** |  **10.32 μs** |   **9.66 μs** |  **1.00** |    **0.00** | **102.5391** |       **-** |       **-** |  **845.33 KB** |        **1.00** |
| UseProvider |  1000 |         10 |      False |    454.6 μs |   5.58 μs |   4.66 μs |  0.56 |    0.01 |  68.8477 |       - |       - |  563.99 KB |        0.67 |
|    UseProxy |  1000 |         10 |      False |    454.9 μs |   8.32 μs |   7.37 μs |  0.56 |    0.01 |  56.6406 |       - |       - |  462.43 KB |        0.55 |
|             |       |            |            |             |           |           |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |         **10** |       **True** | **12,887.0 μs** | **183.94 μs** | **172.05 μs** |  **1.00** |    **0.00** | **156.2500** | **46.8750** | **15.6250** | **1288.03 KB** |        **1.00** |
| UseProvider |  1000 |         10 |       True | 12,993.2 μs | 226.89 μs | 278.65 μs |  1.01 |    0.03 | 140.6250 | 31.2500 | 15.6250 | 1225.06 KB |        0.95 |
|    UseProxy |  1000 |         10 |       True | 12,815.0 μs | 199.93 μs | 187.02 μs |  0.99 |    0.02 | 140.6250 | 46.8750 | 15.6250 | 1154.95 KB |        0.90 |
|             |       |            |            |             |           |           |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |        **100** |      **False** |    **798.5 μs** |  **10.09 μs** |   **9.44 μs** |  **1.00** |    **0.00** | **104.4922** |       **-** |       **-** |  **853.07 KB** |        **1.00** |
| UseProvider |  1000 |        100 |      False |    463.7 μs |   9.15 μs |  10.54 μs |  0.58 |    0.01 |  69.3359 |       - |       - |  571.73 KB |        0.67 |
|    UseProxy |  1000 |        100 |      False |    477.1 μs |   6.68 μs |   5.93 μs |  0.60 |    0.01 |  57.6172 |       - |       - |  470.17 KB |        0.55 |
|             |       |            |            |             |           |           |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |        **100** |       **True** |  **2,704.6 μs** |  **60.80 μs** | **174.44 μs** |  **1.00** |    **0.00** |        **-** |       **-** |       **-** | **1302.54 KB** |        **1.00** |
| UseProvider |  1000 |        100 |       True |  3,079.3 μs |  61.39 μs | 118.28 μs |  1.14 |    0.09 | 156.2500 | 42.9688 |  3.9063 | 1238.49 KB |        0.95 |
|    UseProxy |  1000 |        100 |       True |  3,198.0 μs |  62.85 μs | 125.53 μs |  1.18 |    0.09 | 148.4375 | 42.9688 |  3.9063 | 1167.98 KB |        0.90 |

