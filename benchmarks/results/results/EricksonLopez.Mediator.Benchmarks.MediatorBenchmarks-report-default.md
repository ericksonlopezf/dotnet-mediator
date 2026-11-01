
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Intel Xeon 6975P-C 2.70GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.403
  [Host]    : .NET 10.0.14 (10.0.14, 10.0.1426.52326), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.14 (10.0.14, 10.0.1426.52326), X64 RyuJIT x86-64-v4
  .NET 8.0  : .NET 8.0.33 (8.0.33, 8.0.3326.52219), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4
  ShortRun  : .NET 10.0.14 (10.0.14, 10.0.1426.52326), X64 RyuJIT x86-64-v4


 Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error      | StdDev    | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|-----------:|----------:|-------:|--------:|-------:|----------:|------------:|
 DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.2811 ns |  0.0075 ns | 0.0071 ns |   1.09 |    0.04 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  14.0802 ns |  0.0692 ns | 0.0647 ns |  54.61 |    1.22 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   9.9180 ns |  0.0218 ns | 0.0193 ns |  38.47 |    0.85 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  24.7434 ns |  0.0885 ns | 0.0828 ns |  95.96 |    2.13 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  86.0524 ns |  0.2507 ns | 0.2345 ns | 333.74 |    7.38 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  27.6511 ns |  0.0885 ns | 0.0828 ns | 107.24 |    2.38 | 0.0003 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  70.7450 ns |  0.8985 ns | 0.8404 ns | 274.37 |    6.80 | 0.0014 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  90.4939 ns |  0.6109 ns | 0.5715 ns | 350.96 |    8.00 | 0.0023 |     192 B |          NA |
 NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  41.4538 ns |  0.1401 ns | 0.1311 ns | 160.77 |    3.57 | 0.0002 |      24 B |          NA |
 DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.2580 ns |  0.0063 ns | 0.0059 ns |   1.00 |    0.03 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  18.3206 ns |  0.0702 ns | 0.0657 ns |  71.05 |    1.58 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  16.6259 ns |  0.0947 ns | 0.0886 ns |  64.48 |    1.45 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  34.2017 ns |  0.1149 ns | 0.1075 ns | 132.65 |    2.94 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 174.3063 ns |  0.4360 ns | 0.4079 ns | 676.01 |   14.93 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  37.4444 ns |  0.1162 ns | 0.1087 ns | 145.22 |    3.22 | 0.0002 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  93.2523 ns |  0.3510 ns | 0.3283 ns | 361.66 |    8.04 | 0.0014 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 109.0393 ns |  0.8940 ns | 0.8363 ns | 422.89 |    9.81 | 0.0024 |     200 B |          NA |
 NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  49.9971 ns |  0.6987 ns | 0.6536 ns | 193.90 |    4.92 | 0.0002 |      24 B |          NA |
 DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.2853 ns |  0.0076 ns | 0.0071 ns |   1.11 |    0.04 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  14.4813 ns |  0.0575 ns | 0.0480 ns |  56.16 |    1.25 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  11.6074 ns |  0.0222 ns | 0.0197 ns |  45.02 |    0.99 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  32.3063 ns |  0.1661 ns | 0.1554 ns | 125.29 |    2.81 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 148.7231 ns |  0.5555 ns | 0.5196 ns | 576.79 |   12.82 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  32.9037 ns |  0.1142 ns | 0.1068 ns | 127.61 |    2.83 | 0.0002 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  80.5232 ns |  0.2076 ns | 0.1841 ns | 312.29 |    6.90 | 0.0014 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 105.0684 ns |  0.4400 ns | 0.4116 ns | 407.49 |    9.08 | 0.0023 |     192 B |          NA |
 NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  45.7289 ns |  0.2300 ns | 0.1921 ns | 177.35 |    3.96 | 0.0002 |      24 B |          NA |
 DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.2394 ns |  0.2297 ns | 0.0126 ns |   0.93 |    0.05 |      - |         - |          NA |
 SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  14.7141 ns |  0.1401 ns | 0.0077 ns |  57.07 |    1.27 |      - |         - |          NA |
 SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   9.4219 ns |  0.7226 ns | 0.0396 ns |  36.54 |    0.82 |      - |         - |          NA |
 SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  24.7590 ns |  4.3743 ns | 0.2398 ns |  96.02 |    2.26 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  86.8333 ns |  3.1679 ns | 0.1736 ns | 336.77 |    7.49 |      - |         - |          NA |
 PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  26.7486 ns |  2.3691 ns | 0.1299 ns | 103.74 |    2.34 | 0.0003 |      24 B |          NA |
 PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  67.0369 ns | 30.4094 ns | 1.6668 ns | 259.99 |    7.86 | 0.0014 |     120 B |          NA |
 PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  90.1771 ns |  5.5400 ns | 0.3037 ns | 349.74 |    7.81 | 0.0023 |     192 B |          NA |
 NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  40.0334 ns |  1.7171 ns | 0.0941 ns | 155.26 |    3.45 | 0.0002 |      24 B |          NA |
