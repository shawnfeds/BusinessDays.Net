using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BusinessDays.Net.Abstractions;
using BusinessDays.Net.Core;

namespace BusinessDays.Net.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class BusinessCalendarBenchmarks
    {
        private BusinessCalendar _calendar = null!;
        private BusinessCalendar _calendarNoCache = null!;
        private DateTime _startDate;
        private DateTime _endDate1Year;
        private DateTime _endDate10Years;

        [GlobalSetup]
        public void Setup()
        {
            var holidays = Enumerable.Range(2020, 15)
                .SelectMany(year => new[]
                {
                    new DateTime(year, 1, 1),   // New Year's Day
                    new DateTime(year, 12, 25), // Christmas
                    new DateTime(year, 12, 26)  // Boxing Day
                });

            _calendar = new BusinessCalendar(new FixedHolidayProvider(holidays));
            _calendarNoCache = new BusinessCalendar(new UncachedHolidayProvider(holidays));

            _startDate = new DateTime(2024, 1, 1);
            _endDate1Year = new DateTime(2024, 12, 31);
            _endDate10Years = new DateTime(2034, 12, 31);
        }

        // -------------------------------------------------------------------------
        // IsBusinessDay
        // -------------------------------------------------------------------------

        [Benchmark(Description = "IsBusinessDay - single call")]
        public bool IsBusinessDay()
        {
            return _calendar.IsBusinessDay(_startDate);
        }

        // -------------------------------------------------------------------------
        // AddBusinessDays
        // -------------------------------------------------------------------------

        [Benchmark(Description = "AddBusinessDays - 5 days")]
        public DateTime AddBusinessDays_5()
        {
            return _calendar.AddBusinessDays(_startDate, 5);
        }

        [Benchmark(Description = "AddBusinessDays - 260 days (~1 year)")]
        public DateTime AddBusinessDays_260()
        {
            return _calendar.AddBusinessDays(_startDate, 260);
        }

        // -------------------------------------------------------------------------
        // BusinessDaysBetween — cache vs no cache
        // -------------------------------------------------------------------------

        [Benchmark(Description = "BusinessDaysBetween - 1 year (cached holidays)")]
        public int BusinessDaysBetween_1Year_Cached()
        {
            return _calendar.BusinessDaysBetween(_startDate, _endDate1Year);
        }

        [Benchmark(Description = "BusinessDaysBetween - 1 year (no cache)")]
        public int BusinessDaysBetween_1Year_NoCache()
        {
            return _calendarNoCache.BusinessDaysBetween(_startDate, _endDate1Year);
        }

        [Benchmark(Description = "BusinessDaysBetween - 10 years (cached holidays)")]
        public int BusinessDaysBetween_10Years_Cached()
        {
            return _calendar.BusinessDaysBetween(_startDate, _endDate10Years);
        }

        [Benchmark(Description = "BusinessDaysBetween - 10 years (no cache)")]
        public int BusinessDaysBetween_10Years_NoCache()
        {
            return _calendarNoCache.BusinessDaysBetween(_startDate, _endDate10Years);
        }

        // -------------------------------------------------------------------------
        // NextBusinessDay / PreviousBusinessDay
        // -------------------------------------------------------------------------

        [Benchmark(Description = "NextBusinessDay")]
        public DateTime NextBusinessDay()
        {
            return _calendar.NextBusinessDay(_startDate);
        }

        [Benchmark(Description = "PreviousBusinessDay")]
        public DateTime PreviousBusinessDay()
        {
            return _calendar.PreviousBusinessDay(_startDate);
        }
    }
}