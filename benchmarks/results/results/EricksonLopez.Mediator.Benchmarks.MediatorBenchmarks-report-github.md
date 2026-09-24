```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3
  ShortRun  : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3


```
| Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error      | StdDev    | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|-----------:|----------:|-------:|--------:|-------:|----------:|------------:|
| DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.5994 ns |  0.0050 ns | 0.0042 ns |   2.03 |    0.05 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  21.0367 ns |  0.0298 ns | 0.0264 ns |  71.23 |    1.86 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  15.8107 ns |  0.0244 ns | 0.0216 ns |  53.53 |    1.39 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  47.1858 ns |  0.0573 ns | 0.0508 ns | 159.77 |    4.16 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 172.9931 ns |  0.3466 ns | 0.3072 ns | 585.75 |   15.27 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  41.3709 ns |  0.1875 ns | 0.1663 ns | 140.08 |    3.69 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 199.8679 ns |  0.7231 ns | 0.6764 ns | 676.75 |   17.74 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 183.9473 ns |  0.6840 ns | 0.6063 ns | 622.84 |   16.33 | 0.0114 |     192 B |          NA |
| NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  70.8790 ns |  0.1566 ns | 0.1388 ns | 239.99 |    6.26 | 0.0014 |      24 B |          NA |
| DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.2955 ns |  0.0086 ns | 0.0081 ns |   1.00 |    0.04 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  26.1847 ns |  0.0779 ns | 0.0729 ns |  88.66 |    2.32 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  25.7453 ns |  0.0482 ns | 0.0451 ns |  87.17 |    2.27 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  58.7110 ns |  0.2145 ns | 0.1902 ns | 198.79 |    5.21 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 280.4935 ns |  0.8851 ns | 0.7847 ns | 949.74 |   24.84 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  58.0070 ns |  0.2140 ns | 0.2001 ns | 196.41 |    5.15 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 230.6926 ns |  0.5938 ns | 0.5554 ns | 781.12 |   20.40 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 208.6393 ns |  0.6363 ns | 0.5640 ns | 706.45 |   18.47 | 0.0119 |     200 B |          NA |
| NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  90.2151 ns |  0.1545 ns | 0.1445 ns | 305.47 |    7.96 | 0.0014 |      24 B |          NA |
| DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.3295 ns |  0.0070 ns | 0.0065 ns |   1.12 |    0.04 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  24.8121 ns |  0.0406 ns | 0.0360 ns |  84.01 |    2.19 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  18.7087 ns |  0.0268 ns | 0.0237 ns |  63.35 |    1.65 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  45.8102 ns |  0.0417 ns | 0.0326 ns | 155.11 |    4.04 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 234.2827 ns |  0.1686 ns | 0.1317 ns | 793.27 |   20.65 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  54.3487 ns |  0.1301 ns | 0.1217 ns | 184.02 |    4.80 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 219.8494 ns |  0.7296 ns | 0.6825 ns | 744.40 |   19.49 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 200.4904 ns |  1.4300 ns | 1.3376 ns | 678.86 |   18.20 | 0.0114 |     192 B |          NA |
| NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  79.6508 ns |  0.1610 ns | 0.1344 ns | 269.70 |    7.03 | 0.0014 |      24 B |          NA |
| DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.6005 ns |  0.0334 ns | 0.0018 ns |   2.03 |    0.05 |      - |         - |          NA |
| SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  21.0959 ns |  1.0859 ns | 0.0595 ns |  71.43 |    1.88 |      - |         - |          NA |
| SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  15.6645 ns |  0.7154 ns | 0.0392 ns |  53.04 |    1.40 |      - |         - |          NA |
| SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  47.1593 ns |  1.1149 ns | 0.0611 ns | 159.68 |    4.20 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 183.9336 ns |  0.9138 ns | 0.0501 ns | 622.79 |   16.35 |      - |         - |          NA |
| PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  40.3239 ns |  3.1227 ns | 0.1712 ns | 136.54 |    3.62 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 199.3251 ns |  7.5325 ns | 0.4129 ns | 674.91 |   17.75 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 183.3404 ns | 11.6828 ns | 0.6404 ns | 620.79 |   16.39 | 0.0114 |     192 B |          NA |
| NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  71.7344 ns |  5.1470 ns | 0.2821 ns | 242.89 |    6.42 | 0.0014 |      24 B |          NA |
