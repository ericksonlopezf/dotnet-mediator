```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]    : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.32 (8.0.32, 8.0.3226.50405), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.21 (9.0.21, 9.0.2126.50404), X64 RyuJIT x86-64-v3
  ShortRun  : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3


```
| Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error     | StdDev    | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|----------:|----------:|-------:|--------:|-------:|----------:|------------:|
| DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.6087 ns | 0.0065 ns | 0.0058 ns |   1.82 |    0.05 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  21.1332 ns | 0.0463 ns | 0.0387 ns |  63.05 |    1.68 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  16.1487 ns | 0.0369 ns | 0.0308 ns |  48.18 |    1.28 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  50.3482 ns | 0.0753 ns | 0.0667 ns | 150.22 |    3.99 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 172.8128 ns | 0.2570 ns | 0.2278 ns | 515.61 |   13.69 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  37.9335 ns | 0.0861 ns | 0.0719 ns | 113.18 |    3.01 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 198.8102 ns | 0.4723 ns | 0.4418 ns | 593.18 |   15.78 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 186.2450 ns | 0.8944 ns | 0.7929 ns | 555.69 |   14.92 | 0.0114 |     192 B |          NA |
| NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  75.0634 ns | 0.1572 ns | 0.1313 ns | 223.96 |    5.95 | 0.0014 |      24 B |          NA |
| DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.3354 ns | 0.0107 ns | 0.0095 ns |   1.00 |    0.04 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  25.7460 ns | 0.0401 ns | 0.0355 ns |  76.82 |    2.04 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  24.7483 ns | 0.0667 ns | 0.0591 ns |  73.84 |    1.97 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  55.8746 ns | 0.1233 ns | 0.1093 ns | 166.71 |    4.43 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 255.2308 ns | 0.4045 ns | 0.3586 ns | 761.51 |   20.23 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  57.8710 ns | 0.1731 ns | 0.1534 ns | 172.67 |    4.60 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 228.1156 ns | 1.0559 ns | 0.9360 ns | 680.61 |   18.25 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 152.0108 ns | 0.5804 ns | 0.5145 ns | 453.54 |   12.12 | 0.0119 |     200 B |          NA |
| NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  89.8079 ns | 0.2344 ns | 0.2192 ns | 267.95 |    7.13 | 0.0014 |      24 B |          NA |
| DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.4562 ns | 0.0114 ns | 0.0106 ns |   1.36 |    0.05 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  23.2884 ns | 0.0614 ns | 0.0544 ns |  69.48 |    1.85 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  18.2097 ns | 0.0208 ns | 0.0174 ns |  54.33 |    1.44 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  46.1651 ns | 0.1276 ns | 0.0996 ns | 137.74 |    3.67 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 232.4548 ns | 0.4759 ns | 0.4219 ns | 693.56 |   18.44 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  55.5583 ns | 0.1236 ns | 0.1157 ns | 165.77 |    4.41 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 217.4316 ns | 0.3411 ns | 0.3024 ns | 648.74 |   17.23 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 204.3013 ns | 0.5170 ns | 0.4583 ns | 609.56 |   16.22 | 0.0114 |     192 B |          NA |
| NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  79.3856 ns | 0.1268 ns | 0.1059 ns | 236.86 |    6.29 | 0.0014 |      24 B |          NA |
| DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.2896 ns | 0.1293 ns | 0.0071 ns |   0.86 |    0.03 |      - |         - |          NA |
| SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  20.7689 ns | 0.0870 ns | 0.0048 ns |  61.97 |    1.66 |      - |         - |          NA |
| SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  15.8943 ns | 3.4911 ns | 0.1914 ns |  47.42 |    1.35 |      - |         - |          NA |
| SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  50.8482 ns | 0.5286 ns | 0.0290 ns | 151.71 |    4.06 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 178.7300 ns | 3.5208 ns | 0.1930 ns | 533.26 |   14.29 |      - |         - |          NA |
| PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  39.5182 ns | 7.7732 ns | 0.4261 ns | 117.91 |    3.33 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 201.7520 ns | 2.6442 ns | 0.1449 ns | 601.95 |   16.12 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 187.5748 ns | 2.3119 ns | 0.1267 ns | 559.65 |   14.99 | 0.0114 |     192 B |          NA |
| NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  71.3461 ns | 9.0617 ns | 0.4967 ns | 212.87 |    5.83 | 0.0014 |      24 B |          NA |
