
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v3
  ShortRun  : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3


 Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error     | StdDev    | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|----------:|----------:|-------:|--------:|-------:|----------:|------------:|
 DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.6053 ns | 0.0086 ns | 0.0072 ns |   2.08 |    0.04 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  20.3367 ns | 0.0415 ns | 0.0368 ns |  70.01 |    1.24 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  15.4495 ns | 0.0438 ns | 0.0388 ns |  53.19 |    0.95 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  47.9271 ns | 0.1232 ns | 0.1092 ns | 164.99 |    2.93 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 160.9231 ns | 0.2566 ns | 0.2275 ns | 553.98 |    9.78 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  43.4019 ns | 0.1181 ns | 0.0986 ns | 149.41 |    2.65 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 197.3988 ns | 0.4671 ns | 0.4141 ns | 679.55 |   12.04 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 179.1060 ns | 0.5443 ns | 0.5091 ns | 616.57 |   10.99 | 0.0114 |     192 B |          NA |
 NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  68.2914 ns | 0.0960 ns | 0.0802 ns | 235.09 |    4.15 | 0.0014 |      24 B |          NA |
 DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.2906 ns | 0.0061 ns | 0.0054 ns |   1.00 |    0.03 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  27.3242 ns | 0.0458 ns | 0.0382 ns |  94.06 |    1.66 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  24.7446 ns | 0.0256 ns | 0.0214 ns |  85.18 |    1.50 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  56.5505 ns | 0.1697 ns | 0.1505 ns | 194.68 |    3.46 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 267.9308 ns | 0.3381 ns | 0.2823 ns | 922.35 |   16.27 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  57.9847 ns | 0.2172 ns | 0.2031 ns | 199.61 |    3.58 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 228.3686 ns | 0.7122 ns | 0.5947 ns | 786.16 |   13.98 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 209.4885 ns | 0.6541 ns | 0.5799 ns | 721.17 |   12.84 | 0.0119 |     200 B |          NA |
 NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  93.6316 ns | 0.1661 ns | 0.1554 ns | 322.33 |    5.70 | 0.0014 |      24 B |          NA |
 DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.3333 ns | 0.0058 ns | 0.0051 ns |   1.15 |    0.03 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  26.3041 ns | 0.0645 ns | 0.0538 ns |  90.55 |    1.60 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  18.9097 ns | 0.0136 ns | 0.0114 ns |  65.10 |    1.15 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  45.6123 ns | 0.0563 ns | 0.0470 ns | 157.02 |    2.77 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 225.9047 ns | 0.1683 ns | 0.1492 ns | 777.68 |   13.70 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  54.9572 ns | 0.2523 ns | 0.2360 ns | 189.19 |    3.42 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 211.0503 ns | 0.1737 ns | 0.1356 ns | 726.54 |   12.80 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 196.5146 ns | 0.8955 ns | 0.7478 ns | 676.50 |   12.17 | 0.0114 |     192 B |          NA |
 NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  80.1054 ns | 0.2046 ns | 0.1814 ns | 275.76 |    4.89 | 0.0014 |      24 B |          NA |
 DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.2931 ns | 0.0793 ns | 0.0043 ns |   1.01 |    0.02 |      - |         - |          NA |
 SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  21.7535 ns | 4.0905 ns | 0.2242 ns |  74.89 |    1.48 |      - |         - |          NA |
 SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  15.5482 ns | 0.1784 ns | 0.0098 ns |  53.52 |    0.95 |      - |         - |          NA |
 SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  47.3957 ns | 0.2913 ns | 0.0160 ns | 163.16 |    2.90 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 157.4910 ns | 5.7902 ns | 0.3174 ns | 542.16 |    9.68 |      - |         - |          NA |
 PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  37.0250 ns | 3.1540 ns | 0.1729 ns | 127.46 |    2.32 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 197.4992 ns | 2.3984 ns | 0.1315 ns | 679.89 |   12.09 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 190.2082 ns | 4.8494 ns | 0.2658 ns | 654.79 |   11.66 | 0.0114 |     192 B |          NA |
 NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  66.9795 ns | 9.2091 ns | 0.5048 ns | 230.58 |    4.34 | 0.0014 |      24 B |          NA |
