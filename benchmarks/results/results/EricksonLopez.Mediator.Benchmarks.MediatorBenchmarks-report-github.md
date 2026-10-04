```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
INTEL XEON PLATINUM 8573C 2.30GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v4
  ShortRun  : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error      | StdDev    | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|-----------:|----------:|-------:|--------:|-------:|----------:|------------:|
| DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.2251 ns |  0.0144 ns | 0.0120 ns |   0.65 |    0.04 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  20.2463 ns |  0.1067 ns | 0.0998 ns |  58.09 |    2.26 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  12.4921 ns |  0.0710 ns | 0.0629 ns |  35.84 |    1.39 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  37.7878 ns |  0.2130 ns | 0.1992 ns | 108.42 |    4.22 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 110.9867 ns |  0.2014 ns | 0.1572 ns | 318.44 |   12.30 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  38.7046 ns |  0.1411 ns | 0.1251 ns | 111.05 |    4.30 | 0.0002 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  94.5403 ns |  0.4928 ns | 0.4368 ns | 271.26 |   10.54 | 0.0014 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 117.6833 ns |  0.6929 ns | 0.6142 ns | 337.66 |   13.14 | 0.0021 |     192 B |          NA |
| NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  57.5839 ns |  0.3891 ns | 0.3639 ns | 165.22 |    6.46 | 0.0002 |      24 B |          NA |
| DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.3490 ns |  0.0150 ns | 0.0140 ns |   1.00 |    0.05 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  25.1712 ns |  0.1280 ns | 0.1198 ns |  72.22 |    2.81 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  22.3941 ns |  0.0962 ns | 0.0900 ns |  64.25 |    2.49 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  46.5594 ns |  0.2579 ns | 0.2286 ns | 133.59 |    5.20 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 235.6657 ns |  0.8213 ns | 0.7683 ns | 676.18 |   26.18 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  51.6117 ns |  0.3773 ns | 0.3345 ns | 148.09 |    5.79 | 0.0002 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 121.3972 ns |  0.6954 ns | 0.6505 ns | 348.31 |   13.56 | 0.0014 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 147.7066 ns |  0.5276 ns | 0.4406 ns | 423.80 |   16.41 | 0.0024 |     200 B |          NA |
| NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  72.9361 ns |  0.3608 ns | 0.3198 ns | 209.27 |    8.13 | 0.0002 |      24 B |          NA |
| DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.3768 ns |  0.0240 ns | 0.0225 ns |   1.08 |    0.08 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  20.8690 ns |  0.0933 ns | 0.0779 ns |  59.88 |    2.32 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  15.5344 ns |  0.0819 ns | 0.0766 ns |  44.57 |    1.73 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  42.7380 ns |  0.3076 ns | 0.2878 ns | 122.62 |    4.80 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 185.8502 ns |  0.7703 ns | 0.6829 ns | 533.24 |   20.67 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  48.0869 ns |  0.1487 ns | 0.1391 ns | 137.97 |    5.34 | 0.0002 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 116.1198 ns |  0.4149 ns | 0.3678 ns | 333.17 |   12.90 | 0.0014 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 138.7207 ns |  0.5312 ns | 0.4969 ns | 398.02 |   15.42 | 0.0021 |     192 B |          NA |
| NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  69.0970 ns |  0.4590 ns | 0.4294 ns | 198.25 |    7.74 | 0.0002 |      24 B |          NA |
| DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.5922 ns |  0.5179 ns | 0.0284 ns |   1.70 |    0.09 |      - |         - |          NA |
| SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  19.4844 ns |  3.1734 ns | 0.1739 ns |  55.91 |    2.22 |      - |         - |          NA |
| SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  12.3619 ns |  0.4408 ns | 0.0242 ns |  35.47 |    1.38 |      - |         - |          NA |
| SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  38.6729 ns |  1.3933 ns | 0.0764 ns | 110.96 |    4.33 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 110.2120 ns |  3.1380 ns | 0.1720 ns | 316.22 |   12.32 |      - |         - |          NA |
| PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  39.9201 ns |  2.0807 ns | 0.1140 ns | 114.54 |    4.47 | 0.0002 |      24 B |          NA |
| PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  92.3433 ns |  3.1323 ns | 0.1717 ns | 264.95 |   10.33 | 0.0014 |     120 B |          NA |
| PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 117.2758 ns | 26.4245 ns | 1.4484 ns | 336.49 |   13.55 | 0.0021 |     192 B |          NA |
| NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  55.5180 ns |  8.4331 ns | 0.4622 ns | 159.29 |    6.30 | 0.0002 |      24 B |          NA |
