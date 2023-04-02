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

The `v1.5.0-preview-3`

``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22000.1696/21H2/SunValley)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.202
  [Host]     : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2


```
|      Method | Times | Concurrent | IsUseRedis |        Mean |    Error |    StdDev | Ratio | RatioSD |     Gen0 |    Gen1 |    Gen2 |  Allocated | Alloc Ratio |
|------------ |------ |----------- |----------- |------------:|---------:|----------:|------:|--------:|---------:|--------:|--------:|-----------:|------------:|
| **EasyCaching** |   **500** |         **10** |      **False** |    **381.3 μs** |  **0.67 μs** |   **0.63 μs** |  **1.00** |    **0.00** |  **51.7578** |       **-** |       **-** |  **423.41 KB** |        **1.00** |
| UseProvider |   500 |         10 |      False |    218.9 μs |  1.12 μs |   1.00 μs |  0.57 |    0.00 |  39.7949 |       - |       - |  325.71 KB |        0.77 |
|    UseProxy |   500 |         10 |      False |    221.2 μs |  0.54 μs |   0.48 μs |  0.58 |    0.00 |  28.3203 |       - |       - |  231.96 KB |        0.55 |
|             |       |            |            |             |          |           |       |         |          |         |         |            |             |
| **EasyCaching** |   **500** |         **10** |       **True** |  **6,027.7 μs** | **35.21 μs** |  **32.93 μs** |  **1.00** |    **0.00** |  **78.1250** | **23.4375** |  **7.8125** |  **676.77 KB** |        **1.00** |
| UseProvider |   500 |         10 |       True |  5,890.8 μs | 35.87 μs |  31.80 μs |  0.98 |    0.01 |  54.6875 | 15.6250 |  7.8125 |  489.09 KB |        0.72 |
|    UseProxy |   500 |         10 |       True |  6,132.6 μs | 97.86 μs |  91.54 μs |  1.02 |    0.02 |  70.3125 | 23.4375 |  7.8125 |  579.15 KB |        0.86 |
|             |       |            |            |             |          |           |       |         |          |         |         |            |             |
| **EasyCaching** |   **500** |        **100** |      **False** |    **412.2 μs** |  **6.29 μs** |   **5.89 μs** |  **1.00** |    **0.00** |  **52.7344** |       **-** |       **-** |  **431.15 KB** |        **1.00** |
| UseProvider |   500 |        100 |      False |    248.0 μs |  0.83 μs |   0.74 μs |  0.60 |    0.01 |  40.5273 |       - |       - |  333.45 KB |        0.77 |
|    UseProxy |   500 |        100 |      False |    259.4 μs |  3.34 μs |   3.12 μs |  0.63 |    0.01 |  29.2969 |       - |       - |   239.7 KB |        0.56 |
|             |       |            |            |             |          |           |       |         |          |         |         |            |             |
| **EasyCaching** |   **500** |        **100** |       **True** |  **1,681.5 μs** | **33.51 μs** |  **95.07 μs** |  **1.00** |    **0.00** |  **87.8906** | **23.4375** |  **1.9531** |  **692.57 KB** |        **1.00** |
| UseProvider |   500 |        100 |       True |  1,572.9 μs | 31.40 μs |  91.10 μs |  0.94 |    0.08 |  64.4531 | 17.5781 |  1.9531 |  505.07 KB |        0.73 |
|    UseProxy |   500 |        100 |       True |  1,659.1 μs | 32.31 μs |  84.56 μs |  0.99 |    0.08 |  74.2188 | 21.4844 |  1.9531 |  594.86 KB |        0.86 |
|             |       |            |            |             |          |           |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |         **10** |      **False** |    **716.3 μs** |  **2.33 μs** |   **2.18 μs** |  **1.00** |    **0.00** | **103.5156** |       **-** |       **-** |  **845.33 KB** |        **1.00** |
| UseProvider |  1000 |         10 |      False |    411.2 μs |  1.21 μs |   1.14 μs |  0.57 |    0.00 |  79.5898 |       - |       - |  649.93 KB |        0.77 |
|    UseProxy |  1000 |         10 |      False |    414.3 μs |  1.50 μs |   1.41 μs |  0.58 |    0.00 |  56.6406 |       - |       - |  462.43 KB |        0.55 |
|             |       |            |            |             |          |           |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |         **10** |       **True** | **11,885.7 μs** | **32.01 μs** |  **26.73 μs** |  **1.00** |    **0.00** | **156.2500** | **46.8750** | **15.6250** | **1350.48 KB** |        **1.00** |
| UseProvider |  1000 |         10 |       True | 11,830.2 μs | 77.26 μs |  68.49 μs |  0.99 |    0.01 | 109.3750 | 31.2500 | 15.6250 |  975.96 KB |        0.72 |
|    UseProxy |  1000 |         10 |       True | 12,043.7 μs | 74.87 μs |  70.03 μs |  1.01 |    0.01 | 140.6250 | 46.8750 | 15.6250 | 1155.04 KB |        0.86 |
|             |       |            |            |             |          |           |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |        **100** |      **False** |    **790.7 μs** |  **2.54 μs** |   **2.25 μs** |  **1.00** |    **0.00** | **104.4922** |       **-** |       **-** |  **853.07 KB** |        **1.00** |
| UseProvider |  1000 |        100 |      False |    452.6 μs |  4.34 μs |   3.85 μs |  0.57 |    0.01 |  80.5664 |  0.4883 |       - |  657.67 KB |        0.77 |
|    UseProxy |  1000 |        100 |      False |    439.6 μs |  3.77 μs |   3.34 μs |  0.56 |    0.00 |  57.6172 |       - |       - |  470.17 KB |        0.55 |
|             |       |            |            |             |          |           |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |        **100** |       **True** |  **2,975.4 μs** | **58.74 μs** | **131.39 μs** |  **1.00** |    **0.00** | **171.8750** | **42.9688** |  **3.9063** | **1363.58 KB** |        **1.00** |
| UseProvider |  1000 |        100 |       True |  2,812.0 μs | 55.93 μs |  78.40 μs |  0.95 |    0.05 | 125.0000 | 35.1563 |  3.9063 |  988.14 KB |        0.72 |
|    UseProxy |  1000 |        100 |       True |  2,961.2 μs | 57.93 μs |  79.29 μs |  0.99 |    0.05 | 148.4375 | 42.9688 |  3.9063 | 1168.06 KB |        0.86 |

