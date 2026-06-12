using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run(
    typeof(BusinessDays.Net.Benchmarks.BusinessCalendarBenchmarks),
    DefaultConfig.Instance.WithSummaryStyle(
        BenchmarkDotNet.Reports.SummaryStyle.Default));