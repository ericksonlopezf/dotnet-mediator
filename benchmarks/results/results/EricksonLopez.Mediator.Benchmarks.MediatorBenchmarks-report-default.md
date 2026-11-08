
BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 9V74 3.68GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.403
  [Host]    : .NET 10.0.14 (10.0.14, 10.0.1426.52326), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.14 (10.0.14, 10.0.1426.52326), X64 RyuJIT x86-64-v4
  .NET 8.0  : .NET 8.0.33 (8.0.33, 8.0.3326.52219), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4
  ShortRun  : .NET 10.0.14 (10.0.14, 10.0.1426.52326), X64 RyuJIT x86-64-v4


 Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error      | StdDev    | Median      | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|-----------:|----------:|------------:|-------:|--------:|-------:|----------:|------------:|
 DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.0384 ns |  0.0099 ns | 0.0077 ns |   0.0348 ns |   0.07 |    0.01 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  18.4712 ns |  0.0406 ns | 0.0339 ns |  18.4635 ns |  33.99 |    0.19 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  12.2639 ns |  0.0169 ns | 0.0150 ns |  12.2599 ns |  22.57 |    0.12 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  36.7428 ns |  0.0325 ns | 0.0271 ns |  36.7461 ns |  67.61 |    0.36 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 127.4261 ns |  0.2074 ns | 0.1838 ns | 127.3461 ns | 234.47 |    1.27 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  31.5561 ns |  0.0725 ns | 0.0643 ns |  31.5398 ns |  58.06 |    0.32 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  74.7380 ns |  0.2968 ns | 0.2776 ns |  74.6862 ns | 137.52 |    0.87 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  84.0819 ns |  0.3015 ns | 0.2820 ns |  84.0781 ns | 154.72 |    0.95 | 0.0114 |     192 B |          NA |
 NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  63.2940 ns |  0.1034 ns | 0.0807 ns |  63.2873 ns | 116.46 |    0.62 | 0.0014 |      24 B |          NA |
 DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.5435 ns |  0.0038 ns | 0.0030 ns |   0.5430 ns |   1.00 |    0.01 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  23.3884 ns |  0.0294 ns | 0.0275 ns |  23.3981 ns |  43.04 |    0.23 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  20.6242 ns |  0.0327 ns | 0.0306 ns |  20.6229 ns |  37.95 |    0.21 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  43.7349 ns |  0.0777 ns | 0.0689 ns |  43.7336 ns |  80.47 |    0.44 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 194.4261 ns |  0.8383 ns | 0.7431 ns | 194.0674 ns | 357.75 |    2.29 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  50.4862 ns |  0.1356 ns | 0.1268 ns |  50.4760 ns |  92.90 |    0.53 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 103.3427 ns |  0.4476 ns | 0.4187 ns | 103.3435 ns | 190.16 |    1.24 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 114.4394 ns |  0.3880 ns | 0.3439 ns | 114.4846 ns | 210.57 |    1.26 | 0.0119 |     200 B |          NA |
 NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  70.7723 ns |  0.0718 ns | 0.0600 ns |  70.7709 ns | 130.22 |    0.69 | 0.0014 |      24 B |          NA |
 DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.1860 ns |  0.0668 ns | 0.1724 ns |   0.2721 ns |   0.34 |    0.32 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  20.0164 ns |  0.0730 ns | 0.0683 ns |  20.0047 ns |  36.83 |    0.23 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  14.2617 ns |  0.0211 ns | 0.0187 ns |  14.2577 ns |  26.24 |    0.14 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  41.9128 ns |  0.0652 ns | 0.0578 ns |  41.8935 ns |  77.12 |    0.42 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 170.3319 ns |  0.2820 ns | 0.2638 ns | 170.2882 ns | 313.42 |    1.70 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  45.1679 ns |  0.0871 ns | 0.0772 ns |  45.1545 ns |  83.11 |    0.45 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  95.0219 ns |  0.1957 ns | 0.1830 ns |  94.9898 ns | 174.85 |    0.97 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 104.1990 ns |  0.1695 ns | 0.1503 ns | 104.2370 ns | 191.73 |    1.04 | 0.0114 |     192 B |          NA |
 NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  64.2191 ns |  0.1000 ns | 0.0886 ns |  64.1922 ns | 118.17 |    0.64 | 0.0014 |      24 B |          NA |
 DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.5608 ns |  0.0344 ns | 0.0019 ns |   0.5599 ns |   1.03 |    0.01 |      - |         - |          NA |
 SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  17.4582 ns |  0.4302 ns | 0.0236 ns |  17.4483 ns |  32.12 |    0.17 |      - |         - |          NA |
 SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  12.8317 ns |  0.3079 ns | 0.0169 ns |  12.8287 ns |  23.61 |    0.13 |      - |         - |          NA |
 SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  36.8237 ns |  0.4773 ns | 0.0262 ns |  36.8299 ns |  67.76 |    0.36 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 128.8005 ns | 58.5120 ns | 3.2072 ns | 127.1322 ns | 237.00 |    5.04 |      - |         - |          NA |
 PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  31.9052 ns |  0.8582 ns | 0.0470 ns |  31.9261 ns |  58.71 |    0.32 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  75.7834 ns |  2.2950 ns | 0.1258 ns |  75.7879 ns | 139.45 |    0.76 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  84.1756 ns |  1.8796 ns | 0.1030 ns |  84.1289 ns | 154.89 |    0.83 | 0.0114 |     192 B |          NA |
 NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  63.1807 ns |  3.4606 ns | 0.1897 ns |  63.2046 ns | 116.26 |    0.68 | 0.0014 |      24 B |          NA |
