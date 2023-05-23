# Async

``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2006/22H2/2022Update)
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.203
  [Host]   : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  AOT      : .NET 7.0.5-servicing.23174.5, X64 NativeAOT AVX2
  ShortRun : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2

Platform=X64  Server=True  IterationCount=3  
LaunchCount=1  WarmupCount=3  

```
|      Method |      Job |       Runtime | Times | Concurrent | IsUseRedis |       Mean |       Error |    StdDev | Ratio | RatioSD |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|------------ |--------- |-------------- |------ |----------- |----------- |-----------:|------------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| **EasyCaching** |      **AOT** | **NativeAOT 7.0** |   **500** |         **10** |      **False** |   **788.8 μs** |   **256.15 μs** |  **14.04 μs** |  **1.00** |    **0.00** | **3.9063** | **1.9531** |  **577631 B** |        **1.00** |
| UseProvider |      AOT | NativeAOT 7.0 |   500 |         10 |      False |         NA |          NA |        NA |     ? |       ? |      - |      - |         - |           ? |
|    UseProxy |      AOT | NativeAOT 7.0 |   500 |         10 |      False |   505.5 μs |    54.01 μs |   2.96 μs |  0.64 |    0.01 | 2.4414 | 0.9766 |  349640 B |        0.61 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| EasyCaching | ShortRun |      .NET 7.0 |   500 |         10 |      False |   374.8 μs |    68.59 μs |   3.76 μs |  1.00 |    0.00 | 2.9297 |      - |  433532 B |        1.00 |
| UseProvider | ShortRun |      .NET 7.0 |   500 |         10 |      False |   206.5 μs |    52.78 μs |   2.89 μs |  0.55 |    0.01 | 2.4414 |      - |  333528 B |        0.77 |
|    UseProxy | ShortRun |      .NET 7.0 |   500 |         10 |      False |   273.1 μs |    14.53 μs |   0.80 μs |  0.73 |    0.01 | 1.9531 |      - |  317529 B |        0.73 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| **EasyCaching** |      **AOT** | **NativeAOT 7.0** |   **500** |         **10** |       **True** | **4,198.9 μs** |   **365.75 μs** |  **20.05 μs** |  **1.00** |    **0.00** |      **-** |      **-** |  **693825 B** |        **1.00** |
| UseProvider |      AOT | NativeAOT 7.0 |   500 |         10 |       True |         NA |          NA |        NA |     ? |       ? |      - |      - |         - |           ? |
|    UseProxy |      AOT | NativeAOT 7.0 |   500 |         10 |       True | 4,563.2 μs |   870.28 μs |  47.70 μs |  1.09 |    0.01 |      - |      - |  717886 B |        1.03 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| EasyCaching | ShortRun |      .NET 7.0 |   500 |         10 |       True | 4,040.4 μs |   231.31 μs |  12.68 μs |  1.00 |    0.00 |      - |      - |  669251 B |        1.00 |
| UseProvider | ShortRun |      .NET 7.0 |   500 |         10 |       True | 3,832.2 μs |   719.42 μs |  39.43 μs |  0.95 |    0.01 |      - |      - |  505058 B |        0.75 |
|    UseProxy | ShortRun |      .NET 7.0 |   500 |         10 |       True | 4,249.2 μs |   727.35 μs |  39.87 μs |  1.05 |    0.01 |      - |      - |  685459 B |        1.02 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| **EasyCaching** |      **AOT** | **NativeAOT 7.0** |   **500** |        **100** |      **False** |   **982.0 μs** |   **208.99 μs** |  **11.46 μs** |  **1.00** |    **0.00** | **3.9063** | **1.9531** |  **585552 B** |        **1.00** |
| UseProvider |      AOT | NativeAOT 7.0 |   500 |        100 |      False |         NA |          NA |        NA |     ? |       ? |      - |      - |         - |           ? |
|    UseProxy |      AOT | NativeAOT 7.0 |   500 |        100 |      False |   650.4 μs |   378.61 μs |  20.75 μs |  0.66 |    0.02 | 1.9531 | 0.9766 |  357603 B |        0.61 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| EasyCaching | ShortRun |      .NET 7.0 |   500 |        100 |      False |   400.5 μs |    70.94 μs |   3.89 μs |  1.00 |    0.00 | 2.9297 |      - |  441452 B |        1.00 |
| UseProvider | ShortRun |      .NET 7.0 |   500 |        100 |      False |   231.0 μs |    37.47 μs |   2.05 μs |  0.58 |    0.00 | 2.4414 |      - |  341448 B |        0.77 |
|    UseProxy | ShortRun |      .NET 7.0 |   500 |        100 |      False |   321.1 μs |    55.44 μs |   3.04 μs |  0.80 |    0.01 | 1.9531 |      - |  325481 B |        0.74 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| **EasyCaching** |      **AOT** | **NativeAOT 7.0** |   **500** |        **100** |       **True** | **1,551.0 μs** | **3,731.55 μs** | **204.54 μs** |  **1.00** |    **0.00** | **3.9063** |      **-** |  **709728 B** |        **1.00** |
| UseProvider |      AOT | NativeAOT 7.0 |   500 |        100 |       True |         NA |          NA |        NA |     ? |       ? |      - |      - |         - |           ? |
|    UseProxy |      AOT | NativeAOT 7.0 |   500 |        100 |       True | 1,423.0 μs | 1,853.77 μs | 101.61 μs |  0.93 |    0.16 | 1.9531 |      - |  733915 B |        1.03 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| EasyCaching | ShortRun |      .NET 7.0 |   500 |        100 |       True | 1,316.0 μs |   942.80 μs |  51.68 μs |  1.00 |    0.00 | 1.9531 |      - |  685522 B |        1.00 |
| UseProvider | ShortRun |      .NET 7.0 |   500 |        100 |       True | 1,163.8 μs |   213.79 μs |  11.72 μs |  0.89 |    0.04 | 1.9531 |      - |  522035 B |        0.76 |
|    UseProxy | ShortRun |      .NET 7.0 |   500 |        100 |       True | 1,619.3 μs | 2,518.53 μs | 138.05 μs |  1.23 |    0.15 | 1.9531 |      - |  701660 B |        1.02 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| **EasyCaching** |      **AOT** | **NativeAOT 7.0** |  **1000** |         **10** |      **False** | **2,085.4 μs** | **3,666.69 μs** | **200.98 μs** |  **1.00** |    **0.00** | **7.8125** | **3.9063** | **1153627 B** |        **1.00** |
| UseProvider |      AOT | NativeAOT 7.0 |  1000 |         10 |      False |         NA |          NA |        NA |     ? |       ? |      - |      - |         - |           ? |
|    UseProxy |      AOT | NativeAOT 7.0 |  1000 |         10 |      False |   978.4 μs |   854.54 μs |  46.84 μs |  0.47 |    0.06 | 3.9063 | 1.9531 |  697632 B |        0.60 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| EasyCaching | ShortRun |      .NET 7.0 |  1000 |         10 |      False |   727.3 μs |   279.71 μs |  15.33 μs |  1.00 |    0.00 | 5.8594 |      - |  865538 B |        1.00 |
| UseProvider | ShortRun |      .NET 7.0 |  1000 |         10 |      False |   389.5 μs |    60.92 μs |   3.34 μs |  0.54 |    0.01 | 4.3945 |      - |  665529 B |        0.77 |
|    UseProxy | ShortRun |      .NET 7.0 |  1000 |         10 |      False |   528.2 μs |    68.44 μs |   3.75 μs |  0.73 |    0.01 | 3.9063 |      - |  633530 B |        0.73 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| **EasyCaching** |      **AOT** | **NativeAOT 7.0** |  **1000** |         **10** |       **True** | **8,658.5 μs** | **2,219.45 μs** | **121.66 μs** |  **1.00** |    **0.00** |      **-** |      **-** | **1384561 B** |        **1.00** |
| UseProvider |      AOT | NativeAOT 7.0 |  1000 |         10 |       True |         NA |          NA |        NA |     ? |       ? |      - |      - |         - |           ? |
|    UseProxy |      AOT | NativeAOT 7.0 |  1000 |         10 |       True | 8,751.9 μs | 4,781.20 μs | 262.07 μs |  1.01 |    0.04 |      - |      - | 1433059 B |        1.04 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| EasyCaching | ShortRun |      .NET 7.0 |  1000 |         10 |       True | 8,155.4 μs | 1,988.60 μs | 109.00 μs |  1.00 |    0.00 |      - |      - | 1335660 B |        1.00 |
| UseProvider | ShortRun |      .NET 7.0 |  1000 |         10 |       True | 7,952.7 μs | 2,390.53 μs | 131.03 μs |  0.98 |    0.02 |      - |      - | 1007576 B |        0.75 |
|    UseProxy | ShortRun |      .NET 7.0 |  1000 |         10 |       True | 8,519.8 μs | 2,621.46 μs | 143.69 μs |  1.04 |    0.02 |      - |      - | 1368656 B |        1.02 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| **EasyCaching** |      **AOT** | **NativeAOT 7.0** |  **1000** |        **100** |      **False** | **1,866.1 μs** | **1,060.90 μs** |  **58.15 μs** |  **1.00** |    **0.00** | **7.8125** | **3.9063** | **1161547 B** |        **1.00** |
| UseProvider |      AOT | NativeAOT 7.0 |  1000 |        100 |      False |         NA |          NA |        NA |     ? |       ? |      - |      - |         - |           ? |
|    UseProxy |      AOT | NativeAOT 7.0 |  1000 |        100 |      False | 1,383.2 μs | 2,679.28 μs | 146.86 μs |  0.74 |    0.07 | 3.9063 | 1.9531 |  705553 B |        0.61 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| EasyCaching | ShortRun |      .NET 7.0 |  1000 |        100 |      False |   824.2 μs |   268.46 μs |  14.71 μs |  1.00 |    0.00 | 5.8594 |      - |  873456 B |        1.00 |
| UseProvider | ShortRun |      .NET 7.0 |  1000 |        100 |      False |   473.4 μs | 1,109.36 μs |  60.81 μs |  0.58 |    0.08 | 4.8828 |      - |  673449 B |        0.77 |
|    UseProxy | ShortRun |      .NET 7.0 |  1000 |        100 |      False |   660.0 μs | 1,708.36 μs |  93.64 μs |  0.80 |    0.10 | 3.9063 |      - |  641649 B |        0.73 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| **EasyCaching** |      **AOT** | **NativeAOT 7.0** |  **1000** |        **100** |       **True** | **2,776.7 μs** | **4,673.84 μs** | **256.19 μs** |  **1.00** |    **0.00** | **3.9063** | **3.9063** | **1396686 B** |        **1.00** |
| UseProvider |      AOT | NativeAOT 7.0 |  1000 |        100 |       True |         NA |          NA |        NA |     ? |       ? |      - |      - |         - |           ? |
|    UseProxy |      AOT | NativeAOT 7.0 |  1000 |        100 |       True | 2,902.4 μs | 5,271.72 μs | 288.96 μs |  1.06 |    0.19 | 7.8125 |      - | 1445329 B |        1.03 |
|             |          |               |       |            |            |            |             |           |       |         |        |        |           |             |
| EasyCaching | ShortRun |      .NET 7.0 |  1000 |        100 |       True | 2,305.1 μs | 4,361.56 μs | 239.07 μs |  1.00 |    0.00 | 3.9063 |      - | 1348429 B |        1.00 |
| UseProvider | ShortRun |      .NET 7.0 |  1000 |        100 |       True | 2,179.5 μs | 4,938.12 μs | 270.67 μs |  0.95 |    0.13 | 3.9063 |      - | 1020199 B |        0.76 |
|    UseProxy | ShortRun |      .NET 7.0 |  1000 |        100 |       True | 2,920.8 μs | 3,033.46 μs | 166.27 μs |  1.28 |    0.15 | 3.9063 |      - | 1380461 B |        1.02 |

Benchmarks with issues:
  AutoCacheVsEasyCaching.UseProvider: AOT(Platform=X64, Runtime=NativeAOT 7.0, Server=True, IterationCount=3, LaunchCount=1, WarmupCount=3) [Times=500, Concurrent=10, IsUseRedis=False]
  AutoCacheVsEasyCaching.UseProvider: AOT(Platform=X64, Runtime=NativeAOT 7.0, Server=True, IterationCount=3, LaunchCount=1, WarmupCount=3) [Times=500, Concurrent=10, IsUseRedis=True]
  AutoCacheVsEasyCaching.UseProvider: AOT(Platform=X64, Runtime=NativeAOT 7.0, Server=True, IterationCount=3, LaunchCount=1, WarmupCount=3) [Times=500, Concurrent=100, IsUseRedis=False]
  AutoCacheVsEasyCaching.UseProvider: AOT(Platform=X64, Runtime=NativeAOT 7.0, Server=True, IterationCount=3, LaunchCount=1, WarmupCount=3) [Times=500, Concurrent=100, IsUseRedis=True]
  AutoCacheVsEasyCaching.UseProvider: AOT(Platform=X64, Runtime=NativeAOT 7.0, Server=True, IterationCount=3, LaunchCount=1, WarmupCount=3) [Times=1000, Concurrent=10, IsUseRedis=False]
  AutoCacheVsEasyCaching.UseProvider: AOT(Platform=X64, Runtime=NativeAOT 7.0, Server=True, IterationCount=3, LaunchCount=1, WarmupCount=3) [Times=1000, Concurrent=10, IsUseRedis=True]
  AutoCacheVsEasyCaching.UseProvider: AOT(Platform=X64, Runtime=NativeAOT 7.0, Server=True, IterationCount=3, LaunchCount=1, WarmupCount=3) [Times=1000, Concurrent=100, IsUseRedis=False]
  AutoCacheVsEasyCaching.UseProvider: AOT(Platform=X64, Runtime=NativeAOT 7.0, Server=True, IterationCount=3, LaunchCount=1, WarmupCount=3) [Times=1000, Concurrent=100, IsUseRedis=True]


# Sync

``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2006/22H2/2022Update)
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.203
  [Host]   : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  AOT      : .NET 7.0.5-servicing.23174.5, X64 NativeAOT AVX2
  ShortRun : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2

Platform=X64  Server=True  IterationCount=3  
LaunchCount=1  WarmupCount=3  

```
|      Method |      Job |       Runtime | Times | Concurrent | IsUseRedis |        Mean |        Error |      StdDev | Ratio | RatioSD |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|------------ |--------- |-------------- |------ |----------- |----------- |------------:|-------------:|------------:|------:|--------:|-------:|-------:|----------:|------------:|
| **EasyCaching** |      **AOT** | **NativeAOT 7.0** |  **1000** |        **100** |      **False** |  **1,347.6 μs** |    **118.16 μs** |     **6.48 μs** |  **1.00** |    **0.00** | **5.8594** | **1.9531** |  **936049 B** |        **1.00** |
| UseProvider |      AOT | NativeAOT 7.0 |  1000 |        100 |      False |          NA |           NA |          NA |     ? |       ? |      - |      - |         - |           ? |
|    UseProxy |      AOT | NativeAOT 7.0 |  1000 |        100 |      False |    627.9 μs |    404.03 μs |    22.15 μs |  0.47 |    0.02 | 0.9766 |      - |  392049 B |        0.42 |
|             |          |               |       |            |            |             |              |             |       |         |        |        |           |             |
| EasyCaching | ShortRun |      .NET 7.0 |  1000 |        100 |      False |    495.2 μs |     23.00 μs |     1.26 μs |  1.00 |    0.00 | 3.9063 |      - |  648070 B |        1.00 |
| UseProvider | ShortRun |      .NET 7.0 |  1000 |        100 |      False |    271.7 μs |     13.40 μs |     0.73 μs |  0.55 |    0.00 | 3.4180 |      - |  520065 B |        0.80 |
|    UseProxy | ShortRun |      .NET 7.0 |  1000 |        100 |      False |    386.0 μs |     11.72 μs |     0.64 μs |  0.78 |    0.00 | 1.9531 |      - |  328065 B |        0.51 |
|             |          |               |       |            |            |             |              |             |       |         |        |        |           |             |
| **EasyCaching** |      **AOT** | **NativeAOT 7.0** |  **1000** |        **100** |       **True** | **61,009.5 μs** |  **4,397.75 μs** |   **241.06 μs** |  **1.00** |    **0.00** |      **-** |      **-** |  **752128 B** |        **1.00** |
| UseProvider |      AOT | NativeAOT 7.0 |  1000 |        100 |       True |          NA |           NA |          NA |     ? |       ? |      - |      - |         - |           ? |
|    UseProxy |      AOT | NativeAOT 7.0 |  1000 |        100 |       True | 58,157.3 μs |  4,342.23 μs |   238.01 μs |  0.95 |    0.00 |      - |      - |  592128 B |        0.79 |
|             |          |               |       |            |            |             |              |             |       |         |        |        |           |             |
| EasyCaching | ShortRun |      .NET 7.0 |  1000 |        100 |       True | 63,941.5 μs |  6,429.60 μs |   352.43 μs |  1.00 |    0.00 |      - |      - |  704306 B |        1.00 |
| UseProvider | ShortRun |      .NET 7.0 |  1000 |        100 |       True | 66,670.3 μs |  8,381.80 μs |   459.43 μs |  1.04 |    0.01 |      - |      - |  408422 B |        0.58 |
|    UseProxy | ShortRun |      .NET 7.0 |  1000 |        100 |       True | 67,329.9 μs | 22,953.20 μs | 1,258.14 μs |  1.05 |    0.03 |      - |      - |  528462 B |        0.75 |

Benchmarks with issues:
  AutoCacheVsEasyCachingSync.UseProvider: AOT(Platform=X64, Runtime=NativeAOT 7.0, Server=True, IterationCount=3, LaunchCount=1, WarmupCount=3) [Times=1000, Concurrent=100, IsUseRedis=False]
  AutoCacheVsEasyCachingSync.UseProvider: AOT(Platform=X64, Runtime=NativeAOT 7.0, Server=True, IterationCount=3, LaunchCount=1, WarmupCount=3) [Times=1000, Concurrent=100, IsUseRedis=True]
