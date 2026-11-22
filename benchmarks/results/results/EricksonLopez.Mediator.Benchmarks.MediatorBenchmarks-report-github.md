```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 11.0.100
  [Host]    : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4
  .NET 8.0  : .NET 8.0.34 (8.0.34, 8.0.3426.52217), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4
  ShortRun  : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4


```
| Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error     | StdDev    | Median      | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|----------:|----------:|------------:|-------:|--------:|-------:|----------:|------------:|
| DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.2865 ns | 0.0259 ns | 0.0230 ns |   0.2811 ns |   0.98 |    0.19 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  17.4508 ns | 0.0642 ns | 0.0536 ns |  17.4361 ns |  59.47 |   10.35 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  12.5703 ns | 0.0133 ns | 0.0124 ns |  12.5694 ns |  42.84 |    7.45 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  36.4902 ns | 0.0805 ns | 0.0672 ns |  36.4813 ns | 124.36 |   21.64 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 129.5184 ns | 0.9744 ns | 0.9115 ns | 129.0284 ns | 441.42 |   76.87 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  32.2708 ns | 0.1706 ns | 0.1512 ns |  32.2093 ns | 109.98 |   19.15 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  76.9126 ns | 0.2443 ns | 0.2166 ns |  76.8862 ns | 262.13 |   45.62 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  83.2413 ns | 0.2001 ns | 0.1671 ns |  83.1900 ns | 283.70 |   49.37 | 0.0114 |     192 B |          NA |
| NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  61.4480 ns | 0.1200 ns | 0.1002 ns |  61.4385 ns | 209.42 |   36.45 | 0.0014 |      24 B |          NA |
| DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.3100 ns | 0.0379 ns | 0.0965 ns |   0.2741 ns |   1.06 |    0.38 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  22.9185 ns | 0.1749 ns | 0.1636 ns |  22.8697 ns |  78.11 |   13.60 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  19.8922 ns | 0.0837 ns | 0.0742 ns |  19.8622 ns |  67.80 |   11.80 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  43.5389 ns | 0.4032 ns | 0.3771 ns |  43.3044 ns | 148.39 |   25.85 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 193.8748 ns | 1.3977 ns | 1.3074 ns | 194.1370 ns | 660.75 |  115.06 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  51.2777 ns | 0.3990 ns | 0.3732 ns |  51.1064 ns | 174.76 |   30.44 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 106.9390 ns | 0.8176 ns | 0.7248 ns | 106.6884 ns | 364.46 |   63.47 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 138.7392 ns | 1.2978 ns | 1.2140 ns | 138.5292 ns | 472.84 |   82.38 | 0.0119 |     200 B |          NA |
| NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  73.2038 ns | 0.2072 ns | 0.1836 ns |  73.1840 ns | 249.49 |   43.42 | 0.0014 |      24 B |          NA |
| DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.1989 ns | 0.0148 ns | 0.0115 ns |   0.1928 ns |   0.68 |    0.12 |      - |         - |          NA |
| SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  21.7944 ns | 0.0544 ns | 0.0509 ns |  21.7779 ns |  74.28 |   12.93 |      - |         - |          NA |
| SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  14.3212 ns | 0.0199 ns | 0.0155 ns |  14.3236 ns |  48.81 |    8.49 |      - |         - |          NA |
| SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  42.9246 ns | 0.1493 ns | 0.1323 ns |  42.8627 ns | 146.29 |   25.46 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 178.1475 ns | 0.2104 ns | 0.1757 ns | 178.1390 ns | 607.15 |  105.66 |      - |         - |          NA |
| PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  44.1518 ns | 0.0309 ns | 0.0241 ns |  44.1590 ns | 150.48 |   26.19 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  97.8812 ns | 0.3877 ns | 0.3627 ns |  97.7429 ns | 333.59 |   58.06 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 105.4608 ns | 0.2766 ns | 0.2452 ns | 105.4753 ns | 359.42 |   62.55 | 0.0114 |     192 B |          NA |
| NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  62.8342 ns | 0.2086 ns | 0.1849 ns |  62.8029 ns | 214.15 |   37.27 | 0.0014 |      24 B |          NA |
| DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.2965 ns | 0.1291 ns | 0.0071 ns |   0.2943 ns |   1.01 |    0.18 |      - |         - |          NA |
| SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  17.7290 ns | 2.0699 ns | 0.1135 ns |  17.7784 ns |  60.42 |   10.54 |      - |         - |          NA |
| SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  12.5101 ns | 2.2502 ns | 0.1233 ns |  12.4448 ns |  42.64 |    7.44 |      - |         - |          NA |
| SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  37.3312 ns | 3.6998 ns | 0.2028 ns |  37.3903 ns | 127.23 |   22.19 |      - |         - |          NA |
| SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 136.2696 ns | 9.4473 ns | 0.5178 ns | 136.4436 ns | 464.42 |   80.97 |      - |         - |          NA |
| PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  32.1385 ns | 2.0011 ns | 0.1097 ns |  32.1191 ns | 109.53 |   19.10 | 0.0014 |      24 B |          NA |
| PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  76.4175 ns | 5.1088 ns | 0.2800 ns |  76.4923 ns | 260.44 |   45.41 | 0.0072 |     120 B |          NA |
| PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  84.8407 ns | 1.1057 ns | 0.0606 ns |  84.8068 ns | 289.15 |   50.40 | 0.0114 |     192 B |          NA |
| NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  63.7700 ns | 2.2564 ns | 0.1237 ns |  63.8022 ns | 217.34 |   37.89 | 0.0014 |      24 B |          NA |
