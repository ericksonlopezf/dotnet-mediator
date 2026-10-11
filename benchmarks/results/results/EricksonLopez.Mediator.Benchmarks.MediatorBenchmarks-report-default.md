
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v4
  ShortRun  : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


 Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error      | StdDev    | Median      | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|-----------:|----------:|------------:|-------:|--------:|-------:|----------:|------------:|
 DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.2520 ns |  0.0452 ns | 0.0520 ns |   0.2593 ns |   1.13 |    0.27 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  11.7482 ns |  0.2221 ns | 0.1854 ns |  11.8160 ns |  52.92 |    6.23 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   8.0992 ns |  0.1533 ns | 0.1359 ns |   8.1469 ns |  36.48 |    4.30 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  20.0588 ns |  0.0985 ns | 0.0921 ns |  20.0054 ns |  90.35 |   10.56 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  80.7628 ns |  0.9656 ns | 0.9032 ns |  80.4964 ns | 363.78 |   42.65 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  18.7470 ns |  0.1446 ns | 0.1282 ns |  18.7052 ns |  84.44 |    9.88 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  44.0107 ns |  0.6123 ns | 0.5428 ns |  43.8685 ns | 198.23 |   23.27 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  72.7696 ns |  0.9405 ns | 0.8798 ns |  72.5053 ns | 327.77 |   38.46 | 0.0114 |     192 B |          NA |
 NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  32.5992 ns |  0.6467 ns | 0.6049 ns |  32.3551 ns | 146.84 |   17.35 | 0.0014 |      24 B |          NA |
 DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.2252 ns |  0.0324 ns | 0.0288 ns |   0.2245 ns |   1.01 |    0.17 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  12.4538 ns |  0.2890 ns | 0.2704 ns |  12.5635 ns |  56.10 |    6.66 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  12.6968 ns |  0.2892 ns | 0.3860 ns |  12.5263 ns |  57.19 |    6.89 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  25.5595 ns |  0.2906 ns | 0.2427 ns |  25.5459 ns | 115.13 |   13.49 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 119.8565 ns |  1.0213 ns | 0.8528 ns | 119.3616 ns | 539.86 |   63.16 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  28.3942 ns |  0.5184 ns | 0.4850 ns |  28.1738 ns | 127.89 |   15.08 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  60.9807 ns |  0.5796 ns | 0.4840 ns |  60.8045 ns | 274.67 |   32.15 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  90.5936 ns |  1.7626 ns | 1.5625 ns |  90.5626 ns | 408.06 |   48.13 | 0.0119 |     200 B |          NA |
 NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  38.7177 ns |  0.3338 ns | 0.2606 ns |  38.7394 ns | 174.39 |   20.40 | 0.0014 |      24 B |          NA |
 DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.1559 ns |  0.0150 ns | 0.0125 ns |   0.1576 ns |   0.70 |    0.10 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  11.5736 ns |  0.2736 ns | 0.3041 ns |  11.5331 ns |  52.13 |    6.23 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   9.4176 ns |  0.2061 ns | 0.2751 ns |   9.4200 ns |  42.42 |    5.10 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  24.4542 ns |  0.5195 ns | 0.6935 ns |  24.2594 ns | 110.15 |   13.21 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 107.8141 ns |  0.8320 ns | 0.7376 ns | 107.5620 ns | 485.62 |   56.79 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  31.5427 ns |  0.2760 ns | 0.2582 ns |  31.5580 ns | 142.08 |   16.62 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  56.5371 ns |  1.1479 ns | 2.0699 ns |  55.8096 ns | 254.66 |   31.10 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  82.5358 ns |  1.4790 ns | 1.3111 ns |  82.4014 ns | 371.76 |   43.79 | 0.0114 |     192 B |          NA |
 NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  37.4403 ns |  0.4516 ns | 0.3771 ns |  37.2613 ns | 168.64 |   19.76 | 0.0014 |      24 B |          NA |
 DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.2775 ns |  2.3735 ns | 0.1301 ns |   0.2079 ns |   1.25 |    0.51 |      - |         - |          NA |
 SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  14.4670 ns |  6.3818 ns | 0.3498 ns |  14.4299 ns |  65.16 |    7.79 |      - |         - |          NA |
 SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   8.7041 ns |  1.3347 ns | 0.0732 ns |   8.7018 ns |  39.21 |    4.63 |      - |         - |          NA |
 SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  22.8062 ns |  5.6436 ns | 0.3093 ns |  22.8298 ns | 102.72 |   12.16 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  82.9037 ns | 26.7762 ns | 1.4677 ns |  82.3219 ns | 373.42 |   44.36 |      - |         - |          NA |
 PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  20.1855 ns |  9.1337 ns | 0.5007 ns |  20.3144 ns |  90.92 |   10.88 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  46.6211 ns | 20.9423 ns | 1.1479 ns |  47.2088 ns | 209.99 |   25.12 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  78.1516 ns | 90.9547 ns | 4.9855 ns |  75.5725 ns | 352.01 |   45.51 | 0.0114 |     192 B |          NA |
 NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  35.1439 ns |  8.5360 ns | 0.4679 ns |  34.9705 ns | 158.30 |   18.74 | 0.0014 |      24 B |          NA |
