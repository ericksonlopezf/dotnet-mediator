
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v3
  ShortRun  : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3


 Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error      | StdDev    | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|-----------:|----------:|-------:|--------:|-------:|----------:|------------:|
 DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.6061 ns |  0.0103 ns | 0.0092 ns |   0.99 |    0.02 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  21.0539 ns |  0.0680 ns | 0.0636 ns |  34.45 |    0.35 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  15.7546 ns |  0.0287 ns | 0.0240 ns |  25.78 |    0.25 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  47.0877 ns |  0.0904 ns | 0.0755 ns |  77.05 |    0.75 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 163.2673 ns |  0.4101 ns | 0.3635 ns | 267.15 |    2.64 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  37.6373 ns |  0.0681 ns | 0.0532 ns |  61.58 |    0.60 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 199.7266 ns |  0.4718 ns | 0.4183 ns | 326.80 |    3.23 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 181.2069 ns |  1.5812 ns | 1.4791 ns | 296.50 |    3.70 | 0.0114 |     192 B |          NA |
 NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  68.7805 ns |  0.1849 ns | 0.1544 ns | 112.54 |    1.11 | 0.0014 |      24 B |          NA |
 DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.6112 ns |  0.0074 ns | 0.0062 ns |   1.00 |    0.01 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  27.0237 ns |  0.0243 ns | 0.0215 ns |  44.22 |    0.43 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  25.3520 ns |  0.0555 ns | 0.0464 ns |  41.48 |    0.41 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  55.8241 ns |  0.2491 ns | 0.2080 ns |  91.34 |    0.94 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 259.7655 ns |  0.6673 ns | 0.6242 ns | 425.04 |    4.22 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  57.8397 ns |  0.4263 ns | 0.3988 ns |  94.64 |    1.11 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 221.1035 ns |  1.2717 ns | 1.1273 ns | 361.78 |    3.92 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 208.1069 ns |  1.3436 ns | 1.1220 ns | 340.52 |    3.74 | 0.0119 |     200 B |          NA |
 NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  90.9676 ns |  0.2569 ns | 0.2403 ns | 148.85 |    1.49 | 0.0014 |      24 B |          NA |
 DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.3342 ns |  0.0069 ns | 0.0058 ns |   0.55 |    0.01 |      - |         - |          NA |
 SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  23.4424 ns |  0.0653 ns | 0.0611 ns |  38.36 |    0.38 |      - |         - |          NA |
 SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  18.2463 ns |  0.0235 ns | 0.0196 ns |  29.86 |    0.29 |      - |         - |          NA |
 SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  45.8137 ns |  0.1123 ns | 0.0996 ns |  74.96 |    0.74 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 225.9361 ns |  0.3731 ns | 0.3308 ns | 369.69 |    3.61 |      - |         - |          NA |
 PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  56.0461 ns |  0.2240 ns | 0.1871 ns |  91.71 |    0.93 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 215.4402 ns |  0.6418 ns | 0.6003 ns | 352.51 |    3.53 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 200.1954 ns |  1.0995 ns | 1.0284 ns | 327.57 |    3.56 | 0.0114 |     192 B |          NA |
 NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  78.3878 ns |  0.2898 ns | 0.2569 ns | 128.26 |    1.30 | 0.0014 |      24 B |          NA |
 DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.6072 ns |  0.1473 ns | 0.0081 ns |   0.99 |    0.01 |      - |         - |          NA |
 SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  21.3537 ns |  0.5326 ns | 0.0292 ns |  34.94 |    0.34 |      - |         - |          NA |
 SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  16.1368 ns |  1.9351 ns | 0.1061 ns |  26.40 |    0.29 |      - |         - |          NA |
 SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  47.4314 ns |  0.1918 ns | 0.0105 ns |  77.61 |    0.76 |      - |         - |          NA |
 SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 162.9594 ns |  7.0931 ns | 0.3888 ns | 266.64 |    2.65 |      - |         - |          NA |
 PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  37.7727 ns |  1.5257 ns | 0.0836 ns |  61.81 |    0.61 | 0.0014 |      24 B |          NA |
 PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 198.4963 ns |  6.4914 ns | 0.3558 ns | 324.79 |    3.21 | 0.0072 |     120 B |          NA |
 PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 181.6943 ns | 26.2860 ns | 1.4408 ns | 297.30 |    3.50 | 0.0114 |     192 B |          NA |
 NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  68.6444 ns |  2.8250 ns | 0.1548 ns | 112.32 |    1.12 | 0.0014 |      24 B |          NA |
