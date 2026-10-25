```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]    : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.32 (8.0.32, 8.0.3226.50405), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.21 (9.0.21, 9.0.2126.50404), X64 RyuJIT x86-64-v3
  ShortRun  : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3


```
| Method                           | Job       | Runtime   | IterationCount | LaunchCount | WarmupCount | Mean        | Error     | StdDev    | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |---------- |---------- |--------------- |------------ |------------ |------------:|----------:|----------:|------------:|------:|--------:|-------:|----------:|------------:|
| DirectCall                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |   0.4731 ns | 0.0544 ns | 0.0558 ns |   0.4451 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_NoBehaviors          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  22.1260 ns | 0.0375 ns | 0.0293 ns |  22.1153 ns |     ? |       ? |      - |         - |           ? |
| SendQuery_NoBehaviors            | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  16.8111 ns | 0.0691 ns | 0.0646 ns |  16.7768 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_OneBehavior          | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  47.1941 ns | 0.1218 ns | 0.1017 ns |  47.1561 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_FiveBehaviors        | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 156.7322 ns | 1.3653 ns | 1.2103 ns | 155.9965 ns |     ? |       ? |      - |         - |           ? |
| PublishNotification_OneHandler   | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  41.4494 ns | 0.1168 ns | 0.0912 ns |  41.4321 ns |     ? |       ? | 0.0014 |      24 B |           ? |
| PublishNotification_FiveHandlers | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 101.0739 ns | 0.7467 ns | 0.6619 ns | 100.7261 ns |     ? |       ? | 0.0072 |     120 B |           ? |
| PublishNotification_Parallel     | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     | 110.1080 ns | 0.9682 ns | 0.9056 ns | 109.8538 ns |     ? |       ? | 0.0114 |     192 B |           ? |
| NestedSend                       | .NET 10.0 | .NET 10.0 | Default        | Default     | Default     |  80.9140 ns | 0.2281 ns | 0.2022 ns |  80.8256 ns |     ? |       ? | 0.0014 |      24 B |           ? |
| DirectCall                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |   0.0383 ns | 0.0615 ns | 0.1027 ns |   0.0000 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_NoBehaviors          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  30.1466 ns | 0.0869 ns | 0.0725 ns |  30.1231 ns |     ? |       ? |      - |         - |           ? |
| SendQuery_NoBehaviors            | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  26.1338 ns | 0.0342 ns | 0.0285 ns |  26.1389 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_OneBehavior          | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  59.2371 ns | 0.1609 ns | 0.1256 ns |  59.2627 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_FiveBehaviors        | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 243.6488 ns | 0.6931 ns | 0.5788 ns | 243.4464 ns |     ? |       ? |      - |         - |           ? |
| PublishNotification_OneHandler   | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  67.3293 ns | 0.1942 ns | 0.1721 ns |  67.2737 ns |     ? |       ? | 0.0014 |      24 B |           ? |
| PublishNotification_FiveHandlers | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 139.6337 ns | 0.3788 ns | 0.3358 ns | 139.6789 ns |     ? |       ? | 0.0072 |     120 B |           ? |
| PublishNotification_Parallel     | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     | 147.1457 ns | 0.4877 ns | 0.4323 ns | 147.1578 ns |     ? |       ? | 0.0119 |     200 B |           ? |
| NestedSend                       | .NET 8.0  | .NET 8.0  | Default        | Default     | Default     |  87.5139 ns | 0.2342 ns | 0.1956 ns |  87.5065 ns |     ? |       ? | 0.0014 |      24 B |           ? |
| DirectCall                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |   0.0265 ns | 0.0051 ns | 0.0043 ns |   0.0249 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_NoBehaviors          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  26.1119 ns | 0.0697 ns | 0.0617 ns |  26.1209 ns |     ? |       ? |      - |         - |           ? |
| SendQuery_NoBehaviors            | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  18.5436 ns | 0.0148 ns | 0.0116 ns |  18.5464 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_OneBehavior          | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  54.4528 ns | 0.0466 ns | 0.0364 ns |  54.4481 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_FiveBehaviors        | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 232.6939 ns | 0.5038 ns | 0.4466 ns | 232.6085 ns |     ? |       ? |      - |         - |           ? |
| PublishNotification_OneHandler   | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  55.6089 ns | 0.1576 ns | 0.1397 ns |  55.5725 ns |     ? |       ? | 0.0014 |      24 B |           ? |
| PublishNotification_FiveHandlers | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 132.2890 ns | 0.6800 ns | 0.6028 ns | 132.3309 ns |     ? |       ? | 0.0072 |     120 B |           ? |
| PublishNotification_Parallel     | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     | 134.6746 ns | 0.4331 ns | 0.3839 ns | 134.5773 ns |     ? |       ? | 0.0114 |     192 B |           ? |
| NestedSend                       | .NET 9.0  | .NET 9.0  | Default        | Default     | Default     |  87.9437 ns | 0.2438 ns | 0.2036 ns |  87.8970 ns |     ? |       ? | 0.0014 |      24 B |           ? |
| DirectCall                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |   0.3556 ns | 0.0886 ns | 0.0049 ns |   0.3545 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_NoBehaviors          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  22.4742 ns | 1.2729 ns | 0.0698 ns |  22.4353 ns |     ? |       ? |      - |         - |           ? |
| SendQuery_NoBehaviors            | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  16.0098 ns | 0.0844 ns | 0.0046 ns |  16.0120 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_OneBehavior          | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  47.3961 ns | 0.4612 ns | 0.0253 ns |  47.4057 ns |     ? |       ? |      - |         - |           ? |
| SendCommand_FiveBehaviors        | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 149.0542 ns | 0.9598 ns | 0.0526 ns | 149.0400 ns |     ? |       ? |      - |         - |           ? |
| PublishNotification_OneHandler   | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  41.4828 ns | 3.8648 ns | 0.2118 ns |  41.3794 ns |     ? |       ? | 0.0014 |      24 B |           ? |
| PublishNotification_FiveHandlers | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  98.5127 ns | 3.0855 ns | 0.1691 ns |  98.5517 ns |     ? |       ? | 0.0072 |     120 B |           ? |
| PublishNotification_Parallel     | ShortRun  | .NET 10.0 | 3              | 1           | 3           | 112.0745 ns | 6.3568 ns | 0.3484 ns | 111.9501 ns |     ? |       ? | 0.0114 |     192 B |           ? |
| NestedSend                       | ShortRun  | .NET 10.0 | 3              | 1           | 3           |  81.9967 ns | 2.3422 ns | 0.1284 ns |  81.9822 ns |     ? |       ? | 0.0014 |      24 B |           ? |
