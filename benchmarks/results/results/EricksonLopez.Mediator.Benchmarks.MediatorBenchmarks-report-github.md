```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Intel Xeon 6973P-C 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v4
  ShortRun  : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error      | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|-----------:|----------:|------:|--------:|-------:|----------:|------------:|
| DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.1987 ns |  0.0026 ns | 0.0024 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  14.4257 ns |  0.0223 ns | 0.0186 ns |     ? |       ? |      - |         - |           ? |
| SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   9.0800 ns |  0.2251 ns | 0.2106 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  24.8764 ns |  0.1867 ns | 0.1559 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  80.3592 ns |  0.1594 ns | 0.1413 ns |     ? |       ? |      - |         - |           ? |
| PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  25.5074 ns |  0.1838 ns | 0.1630 ns |     ? |       ? | 0.0003 |      24 B |           ? |
| PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  60.1439 ns |  0.8084 ns | 0.7562 ns |     ? |       ? | 0.0014 |     120 B |           ? |
| PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  83.8478 ns |  1.6746 ns | 1.9935 ns |     ? |       ? | 0.0023 |     192 B |           ? |
| NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  39.1103 ns |  0.7780 ns | 0.7641 ns |     ? |       ? | 0.0002 |      24 B |           ? |
| DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.0000 ns |  0.0000 ns | 0.0000 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  16.5219 ns |  0.2307 ns | 0.2266 ns |     ? |       ? |      - |         - |           ? |
| SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  15.6972 ns |  0.2931 ns | 0.2599 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  32.9014 ns |  0.7010 ns | 0.9827 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 160.9145 ns |  0.1511 ns | 0.1262 ns |     ? |       ? |      - |         - |           ? |
| PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  33.8568 ns |  0.1561 ns | 0.1218 ns |     ? |       ? | 0.0002 |      24 B |           ? |
| PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  81.2412 ns |  1.5346 ns | 1.4355 ns |     ? |       ? | 0.0014 |     120 B |           ? |
| PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 101.6366 ns |  1.9847 ns | 2.1236 ns |     ? |       ? | 0.0024 |     200 B |           ? |
| NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  48.5839 ns |  0.4513 ns | 0.3523 ns |     ? |       ? | 0.0002 |      24 B |           ? |
| DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.5261 ns |  0.0590 ns | 0.0552 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  13.7428 ns |  0.0167 ns | 0.0130 ns |     ? |       ? |      - |         - |           ? |
| SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  10.6257 ns |  0.0193 ns | 0.0180 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  30.4529 ns |  0.1191 ns | 0.0930 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 132.1860 ns |  0.2331 ns | 0.1820 ns |     ? |       ? |      - |         - |           ? |
| PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  32.2915 ns |  0.1868 ns | 0.1560 ns |     ? |       ? | 0.0002 |      24 B |           ? |
| PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  74.8836 ns |  0.1124 ns | 0.0939 ns |     ? |       ? | 0.0014 |     120 B |           ? |
| PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  94.9476 ns |  0.8667 ns | 0.7237 ns |     ? |       ? | 0.0023 |     192 B |           ? |
| NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  43.5630 ns |  0.1444 ns | 0.1280 ns |     ? |       ? | 0.0002 |      24 B |           ? |
| DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.2354 ns |  0.1614 ns | 0.0088 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  12.8053 ns |  0.3511 ns | 0.0192 ns |     ? |       ? |      - |         - |           ? |
| SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   8.9651 ns |  2.1229 ns | 0.1164 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  22.2075 ns |  0.4000 ns | 0.0219 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  88.1204 ns |  0.7885 ns | 0.0432 ns |     ? |       ? |      - |         - |           ? |
| PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  26.3934 ns | 23.6359 ns | 1.2956 ns |     ? |       ? | 0.0003 |      24 B |           ? |
| PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  61.7896 ns |  9.1247 ns | 0.5002 ns |     ? |       ? | 0.0014 |     120 B |           ? |
| PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  83.0303 ns |  4.4328 ns | 0.2430 ns |     ? |       ? | 0.0023 |     192 B |           ? |
| NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  42.0093 ns |  0.8457 ns | 0.0464 ns |     ? |       ? | 0.0002 |      24 B |           ? |
