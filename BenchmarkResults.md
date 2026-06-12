```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8655/25H2/2025Update/HudsonValley2)
Intel Core 5 210H 2.20GHz, 1 CPU, 12 logical and 8 physical cores
.NET SDK 10.0.300
  [Host]     : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3


```
| Method                                             | Mean         | Error        | StdDev       | Rank | Allocated |
|--------------------------------------------------- |-------------:|-------------:|-------------:|-----:|----------:|
| &#39;IsBusinessDay - single call&#39;                      |     16.55 ns |     0.139 ns |     0.124 ns |    1 |         - |
| NextBusinessDay                                    |     18.44 ns |     0.190 ns |     0.169 ns |    2 |         - |
| PreviousBusinessDay                                |     24.28 ns |     0.123 ns |     0.115 ns |    3 |         - |
| &#39;AddBusinessDays - 5 days&#39;                         |     91.36 ns |     1.810 ns |     1.859 ns |    4 |         - |
| &#39;AddBusinessDays - 260 days (~1 year)&#39;             |  4,833.59 ns |    81.797 ns |    76.513 ns |    5 |         - |
| &#39;BusinessDaysBetween - 1 year (no cache)&#39;          |  4,901.97 ns |    94.150 ns |   108.423 ns |    5 |         - |
| &#39;BusinessDaysBetween - 1 year (cached holidays)&#39;   |  4,924.46 ns |    84.786 ns |    79.309 ns |    5 |         - |
| &#39;BusinessDaysBetween - 10 years (cached holidays)&#39; | 58,842.53 ns |   792.287 ns |   741.106 ns |    6 |         - |
| &#39;BusinessDaysBetween - 10 years (no cache)&#39;        | 60,233.19 ns | 1,072.554 ns | 1,003.268 ns |    6 |         - |
