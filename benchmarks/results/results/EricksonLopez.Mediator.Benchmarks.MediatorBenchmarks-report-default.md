
BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
Intel Xeon 6975P-C 2.70GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 11.0.100
  [Host]    : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4
  .NET 8.0  : .NET 8.0.34 (8.0.34, 8.0.3426.52217), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4
  ShortRun  : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4


 Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error     | StdDev    | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|----------:|----------:|-------:|--------:|-------:|----------:|------------:|
 DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.2729 ns | 0.0093 ns | 0.0082 ns |   1.10 |    0.04 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  14.1800 ns | 0.0317 ns | 0.0281 ns |  57.18 |    0.76 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   9.4235 ns | 0.0525 ns | 0.0439 ns |  38.00 |    0.53 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  24.7445 ns | 0.1497 ns | 0.1401 ns |  99.78 |    1.42 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  85.8553 ns | 0.5510 ns | 0.5154 ns | 346.20 |    4.98 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  26.7051 ns | 0.0945 ns | 0.0838 ns | 107.69 |    1.45 | 0.0003 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  71.6616 ns | 0.3517 ns | 0.3117 ns | 288.97 |    3.99 | 0.0014 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  93.5857 ns | 0.3419 ns | 0.2855 ns | 377.38 |    5.09 | 0.0023 |     192 B |          NA |
 NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  41.1485 ns | 0.2529 ns | 0.2365 ns | 165.93 |    2.37 | 0.0002 |      24 B |          NA |
 DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.2480 ns | 0.0041 ns | 0.0034 ns |   1.00 |    0.02 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  17.5151 ns | 0.1462 ns | 0.1367 ns |  70.63 |    1.07 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  15.8756 ns | 0.0767 ns | 0.0717 ns |  64.02 |    0.89 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  34.6154 ns | 0.1938 ns | 0.1813 ns | 139.58 |    1.97 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 173.8135 ns | 0.6530 ns | 0.6108 ns | 700.89 |    9.53 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  36.8963 ns | 0.0801 ns | 0.0710 ns | 148.78 |    1.98 | 0.0002 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  90.0545 ns | 0.4362 ns | 0.4080 ns | 363.14 |    5.04 | 0.0014 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 109.6273 ns | 0.5042 ns | 0.4716 ns | 442.06 |    6.10 | 0.0024 |     200 B |          NA |
 NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  53.1482 ns | 0.1293 ns | 0.1209 ns | 214.32 |    2.86 | 0.0002 |      24 B |          NA |
 DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.5386 ns | 0.0095 ns | 0.0089 ns |   2.17 |    0.05 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  14.3370 ns | 0.0268 ns | 0.0209 ns |  57.81 |    0.77 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  11.6196 ns | 0.0481 ns | 0.0427 ns |  46.86 |    0.64 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  32.0693 ns | 0.2176 ns | 0.2036 ns | 129.32 |    1.88 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 163.8093 ns | 0.7589 ns | 0.7099 ns | 660.55 |    9.12 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  32.5431 ns | 0.0944 ns | 0.0883 ns | 131.23 |    1.76 | 0.0002 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  77.5900 ns | 0.3899 ns | 0.3647 ns | 312.87 |    4.36 | 0.0014 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 100.4064 ns | 0.4793 ns | 0.4483 ns | 404.88 |    5.61 | 0.0023 |     192 B |          NA |
 NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  45.6962 ns | 0.3344 ns | 0.3128 ns | 184.27 |    2.72 | 0.0002 |      24 B |          NA |
 DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.2510 ns | 0.0424 ns | 0.0023 ns |   1.01 |    0.02 |      - |         - |          NA |
 SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  13.2092 ns | 1.0660 ns | 0.0584 ns |  53.26 |    0.73 |      - |         - |          NA |
 SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   9.4235 ns | 0.9765 ns | 0.0535 ns |  38.00 |    0.54 |      - |         - |          NA |
 SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  26.7430 ns | 0.6183 ns | 0.0339 ns | 107.84 |    1.44 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  88.7795 ns | 6.9184 ns | 0.3792 ns | 358.00 |    4.93 |      - |         - |          NA |
 PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  28.4797 ns | 2.6075 ns | 0.1429 ns | 114.84 |    1.60 | 0.0002 |      24 B |          NA |
 PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  68.6622 ns | 1.9547 ns | 0.1071 ns | 276.87 |    3.70 | 0.0014 |     120 B |          NA |
 PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  92.6580 ns | 6.7646 ns | 0.3708 ns | 373.64 |    5.12 | 0.0023 |     192 B |          NA |
 NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  46.2110 ns | 4.3126 ns | 0.2364 ns | 186.34 |    2.60 | 0.0002 |      24 B |          NA |
