using System;
using System.Collections.Generic;
using System.Linq;
using BusinessDays.Net.Abstractions;

namespace BusinessDays.Net.Benchmarks
{
    /// <summary>
    /// Returns a fixed list — simulates a warmed, in-memory provider.
    /// </summary>
    internal sealed class FixedHolidayProvider : IHolidayProvider
    {
        private readonly List<DateTime> _holidays;

        public FixedHolidayProvider(IEnumerable<DateTime> holidays)
        {
            _holidays = holidays.ToList();
        }

        public IEnumerable<DateTime> GetHolidays(int year)
            => _holidays.Where(d => d.Year == year);
    }

    /// <summary>
    /// Re-enumerates on every call — simulates an uncached provider
    /// (e.g. hitting a database or doing LINQ over a large list).
    /// This is what BusinessCalendar's internal cache protects against.
    /// </summary>
    internal sealed class UncachedHolidayProvider : IHolidayProvider
    {
        private readonly List<DateTime> _holidays;

        public UncachedHolidayProvider(IEnumerable<DateTime> holidays)
        {
            _holidays = holidays.ToList();
        }

        public IEnumerable<DateTime> GetHolidays(int year)
        {
            // Simulate work on every call — defeat any caller-side caching
            return _holidays
                .Where(d => d.Year == year)
                .Select(d => d); // force re-enumeration
        }
    }
}