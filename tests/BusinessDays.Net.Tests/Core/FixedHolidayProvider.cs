using System;
using System.Collections.Generic;
using BusinessDays.Net.Abstractions;

namespace BusinessDays.Net.Tests
{
    /// <summary>
    /// Test double — returns a fixed list of holidays for any year.
    /// </summary>
    internal sealed class FixedHolidayProvider : IHolidayProvider
    {
        private readonly IEnumerable<DateTime> _holidays;

        public FixedHolidayProvider(IEnumerable<DateTime> holidays)
        {
            _holidays = holidays;
        }

        public IEnumerable<DateTime> GetHolidays(int year) => _holidays;
    }
}