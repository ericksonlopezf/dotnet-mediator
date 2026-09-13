
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3
  ShortRun  : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3


 Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error      | StdDev    | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|-----------:|----------:|-------:|--------:|-------:|----------:|------------:|
 DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.6067 ns |  0.0092 ns | 0.0077 ns |   1.98 |    0.24 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  21.6741 ns |  0.0750 ns | 0.0626 ns |  70.85 |    8.37 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  16.0288 ns |  0.0280 ns | 0.0248 ns |  52.40 |    6.19 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  53.7074 ns |  0.1752 ns | 0.1553 ns | 175.57 |   20.74 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 188.0161 ns |  0.1737 ns | 0.1450 ns | 614.63 |   72.61 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  37.5814 ns |  0.2560 ns | 0.2270 ns | 122.85 |   14.53 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 198.6224 ns |  1.6094 ns | 1.5054 ns | 649.30 |   76.83 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 188.4771 ns |  1.4472 ns | 1.3537 ns | 616.13 |   72.89 | 0.0114 |     192 B |          NA |
 NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  69.9468 ns |  0.1932 ns | 0.1613 ns | 228.66 |   27.02 | 0.0014 |      24 B |          NA |
 DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.3115 ns |  0.0590 ns | 0.0492 ns |   1.02 |    0.20 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  26.2453 ns |  0.0410 ns | 0.0363 ns |  85.80 |   10.13 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  25.8076 ns |  0.0336 ns | 0.0262 ns |  84.37 |    9.97 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  55.7513 ns |  0.0592 ns | 0.0494 ns | 182.25 |   21.53 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 261.9612 ns |  1.1512 ns | 1.0205 ns | 856.35 |  101.20 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  58.5781 ns |  0.0621 ns | 0.0518 ns | 191.49 |   22.62 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 224.9574 ns |  0.4922 ns | 0.4604 ns | 735.39 |   86.86 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 211.5244 ns |  0.9037 ns | 0.8453 ns | 691.48 |   81.70 | 0.0119 |     200 B |          NA |
 NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  90.8799 ns |  0.4615 ns | 0.4317 ns | 297.09 |   35.11 | 0.0014 |      24 B |          NA |
 DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.3299 ns |  0.0048 ns | 0.0042 ns |   1.08 |    0.13 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  23.4310 ns |  0.0521 ns | 0.0435 ns |  76.60 |    9.05 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  19.9581 ns |  0.0163 ns | 0.0145 ns |  65.24 |    7.71 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  45.6103 ns |  0.0608 ns | 0.0539 ns | 149.10 |   17.61 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 223.5688 ns |  0.5824 ns | 0.5162 ns | 730.85 |   86.34 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  55.2021 ns |  0.2886 ns | 0.2699 ns | 180.46 |   21.33 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 220.5391 ns |  0.8472 ns | 0.7925 ns | 720.94 |   85.18 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 200.5843 ns |  1.3778 ns | 1.2213 ns | 655.71 |   77.55 | 0.0114 |     192 B |          NA |
 NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  79.9200 ns |  0.1446 ns | 0.1352 ns | 261.26 |   30.86 | 0.0014 |      24 B |          NA |
 DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.6030 ns |  0.1455 ns | 0.0080 ns |   1.97 |    0.24 |      - |         - |          NA |
 SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  21.0641 ns |  0.1868 ns | 0.0102 ns |  68.86 |    8.22 |      - |         - |          NA |
 SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  15.4844 ns |  0.1111 ns | 0.0061 ns |  50.62 |    6.04 |      - |         - |          NA |
 SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  47.8115 ns |  0.3359 ns | 0.0184 ns | 156.30 |   18.65 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 188.6092 ns |  9.4296 ns | 0.5169 ns | 616.57 |   73.59 |      - |         - |          NA |
 PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  37.0864 ns |  2.1382 ns | 0.1172 ns | 121.24 |   14.47 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 198.0651 ns | 24.2050 ns | 1.3268 ns | 647.48 |   77.35 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 182.1086 ns | 27.1003 ns | 1.4855 ns | 595.32 |   71.15 | 0.0114 |     192 B |          NA |
 NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  74.2519 ns | 34.9315 ns | 1.9147 ns | 242.73 |   29.43 | 0.0014 |      24 B |          NA |
