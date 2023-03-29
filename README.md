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

``` ini

BenchmarkDotNet=v0.13.4, OS=Windows 10 (10.0.19045.2728)
Intel Core i5-9400F CPU 2.90GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK=8.0.100-preview.1.23115.2
  [Host]     : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2


```
|      Method | Times | Concurrent | IsUseRedis |        Mean |     Error |    StdDev |      Median | Ratio | RatioSD |     Gen0 |    Gen1 |    Gen2 |  Allocated | Alloc Ratio |
|------------ |------ |----------- |----------- |------------:|----------:|----------:|------------:|------:|--------:|---------:|--------:|--------:|-----------:|------------:|
| **EasyCaching** |   **500** |         **10** |      **False** |    **389.0 μs** |   **6.06 μs** |   **5.66 μs** |    **390.8 μs** |  **1.00** |    **0.00** |  **91.7969** |       **-** |       **-** |  **423.45 KB** |        **1.00** |
| UseProvider |   500 |         10 |      False |    273.1 μs |   5.33 μs |   4.99 μs |    270.6 μs |  0.70 |    0.01 |  71.7773 |       - |       - |  329.62 KB |        0.78 |
|    UseProxy |   500 |         10 |      False |    240.8 μs |   3.71 μs |   3.29 μs |    240.0 μs |  0.62 |    0.01 |  63.9648 |       - |       - |  294.46 KB |        0.70 |
|             |       |            |            |             |           |           |             |       |         |          |         |         |            |             |
| **EasyCaching** |   **500** |         **10** |       **True** |  **5,370.1 μs** | **107.03 μs** | **175.85 μs** |  **5,335.6 μs** |  **1.00** |    **0.00** | **117.1875** | **31.2500** |  **7.8125** |  **646.47 KB** |        **1.00** |
| UseProvider |   500 |         10 |       True |  5,699.8 μs | 127.29 μs | 371.30 μs |  5,660.0 μs |  1.08 |    0.08 | 117.1875 | 31.2500 |  7.8125 |  681.54 KB |        1.05 |
|    UseProxy |   500 |         10 |       True |  5,274.3 μs |  99.90 μs | 102.59 μs |  5,252.4 μs |  0.97 |    0.04 | 109.3750 | 31.2500 |  7.8125 |  634.62 KB |        0.98 |
|             |       |            |            |             |           |           |             |       |         |          |         |         |            |             |
| **EasyCaching** |   **500** |        **100** |      **False** |    **416.1 μs** |   **6.80 μs** |   **6.36 μs** |    **416.2 μs** |  **1.00** |    **0.00** |  **93.7500** |  **0.4883** |       **-** |  **431.18 KB** |        **1.00** |
| UseProvider |   500 |        100 |      False |    296.5 μs |   5.77 μs |   5.67 μs |    298.1 μs |  0.71 |    0.02 |  73.2422 |  0.4883 |       - |  337.35 KB |        0.78 |
|    UseProxy |   500 |        100 |      False |    275.5 μs |   5.37 μs |   6.40 μs |    276.6 μs |  0.66 |    0.02 |  65.4297 |       - |       - |  302.19 KB |        0.70 |
|             |       |            |            |             |           |           |             |       |         |          |         |         |            |             |
| **EasyCaching** |   **500** |        **100** |       **True** |  **1,813.9 μs** |  **68.97 μs** | **203.35 μs** |  **1,802.8 μs** |  **1.00** |    **0.00** |        **-** |       **-** |       **-** |   **664.4 KB** |        **1.00** |
| UseProvider |   500 |        100 |       True |  1,658.8 μs |  31.82 μs |  36.64 μs |  1,651.6 μs |  0.90 |    0.07 | 134.7656 | 42.9688 |  1.9531 |  698.72 KB |        1.05 |
|    UseProxy |   500 |        100 |       True |  1,598.0 μs |  31.16 μs |  34.63 μs |  1,601.3 μs |  0.86 |    0.06 | 123.0469 | 35.1563 |  1.9531 |  651.56 KB |        0.98 |
|             |       |            |            |             |           |           |             |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |         **10** |      **False** |    **774.3 μs** |   **9.91 μs** |   **9.27 μs** |    **778.1 μs** |  **1.00** |    **0.00** | **183.5938** |       **-** |       **-** |   **845.4 KB** |        **1.00** |
| UseProvider |  1000 |         10 |      False |    524.9 μs |   6.33 μs |   5.92 μs |    526.9 μs |  0.68 |    0.01 | 142.5781 |       - |       - |  657.74 KB |        0.78 |
|    UseProxy |  1000 |         10 |      False |    488.2 μs |   5.51 μs |   5.15 μs |    486.8 μs |  0.63 |    0.01 | 127.9297 |       - |       - |  587.43 KB |        0.69 |
|             |       |            |            |             |           |           |             |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |         **10** |       **True** | **11,353.6 μs** | **224.86 μs** | **488.82 μs** | **11,236.7 μs** |  **1.00** |    **0.00** | **234.3750** | **62.5000** | **15.6250** | **1289.82 KB** |        **1.00** |
| UseProvider |  1000 |         10 |       True | 11,095.7 μs | 215.99 μs | 483.08 μs | 11,053.7 μs |  0.98 |    0.06 | 234.3750 | 62.5000 | 15.6250 |  1360.4 KB |        1.05 |
|    UseProxy |  1000 |         10 |       True | 11,089.5 μs | 221.39 μs | 526.17 μs | 10,916.8 μs |  0.97 |    0.05 | 218.7500 | 62.5000 | 15.6250 | 1266.14 KB |        0.98 |
|             |       |            |            |             |           |           |             |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |        **100** |      **False** |    **800.8 μs** |   **6.17 μs** |   **5.77 μs** |    **798.7 μs** |  **1.00** |    **0.00** | **185.5469** |  **0.9766** |       **-** |  **853.13 KB** |        **1.00** |
| UseProvider |  1000 |        100 |      False |    555.9 μs |   5.08 μs |   4.51 μs |    556.2 μs |  0.69 |    0.01 | 144.5313 |       - |       - |  665.48 KB |        0.78 |
|    UseProxy |  1000 |        100 |      False |    504.8 μs |   5.18 μs |   4.85 μs |    504.6 μs |  0.63 |    0.01 | 128.9063 |       - |       - |  595.17 KB |        0.70 |
|             |       |            |            |             |           |           |             |       |         |          |         |         |            |             |
| **EasyCaching** |  **1000** |        **100** |       **True** |  **3,167.6 μs** | **117.50 μs** | **344.62 μs** |  **3,164.0 μs** |  **1.00** |    **0.00** |        **-** |       **-** |       **-** | **1309.46 KB** |        **1.00** |
| UseProvider |  1000 |        100 |       True |  3,140.8 μs |  62.55 μs | 107.90 μs |  3,122.2 μs |  0.99 |    0.12 | 261.7188 | 78.1250 |  3.9063 | 1375.95 KB |        1.05 |
|    UseProxy |  1000 |        100 |       True |  3,028.6 μs |  57.94 μs | 123.48 μs |  3,024.7 μs |  0.99 |    0.11 | 242.1875 | 70.3125 |  3.9063 | 1281.96 KB |        0.98 |
