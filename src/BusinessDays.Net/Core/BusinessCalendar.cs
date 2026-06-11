using System;
using System.Collections.Generic;
using System.Linq;
using BusinessDays.Net.Abstractions;

namespace BusinessDays.Net.Core
{
    /// <summary>
    /// Default implementation of IBusinessCalendar.
    /// Thread-safe for reads after construction.
    /// </summary>
    public sealed class BusinessCalendar : IBusinessCalendar
    {
        private readonly IHolidayProvider _holidayProvider;
        private readonly HashSet<DayOfWeek> _workingDays;
        private readonly Dictionary<int, HashSet<DateTime>> _holidayCache;
        private readonly object _cacheLock = new object();

        /// <summary>
        /// Standard Mon–Fri calendar with no holidays.
        /// </summary>
        public static readonly BusinessCalendar Default = new BusinessCalendar();

        /// <summary>
        /// Creates a calendar with default Mon–Fri working week and no holidays.
        /// </summary>
        public BusinessCalendar()
            : this(NullHolidayProvider.Instance, new[]
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday
            })
        { }

        /// <summary>
        /// Creates a calendar with a custom working week and no holidays.
        /// </summary>
        public BusinessCalendar(DayOfWeek[] workingDays): this(NullHolidayProvider.Instance, workingDays)
        { 
        }

        /// <summary>
        /// Creates a calendar with Mon–Fri working week and a custom holiday provider.
        /// </summary>
        public BusinessCalendar(IHolidayProvider holidayProvider): this(holidayProvider, new[]
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday
            })
        { 
        }

        /// <summary>
        /// Full constructor.
        /// </summary>
        public BusinessCalendar(IHolidayProvider holidayProvider, DayOfWeek[] workingDays)
        {
            if (holidayProvider == null) 
                throw new ArgumentNullException(nameof(holidayProvider));

            if (workingDays == null || workingDays.Length == 0)
                throw new ArgumentException("Working days must contain at least one day.", nameof(workingDays));

            _holidayProvider = holidayProvider;
            _workingDays = new HashSet<DayOfWeek>(workingDays);
            _holidayCache = new Dictionary<int, HashSet<DateTime>>();
        }

        // -------------------------------------------------------------------------
        // Core
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public bool IsBusinessDay(DateTime date)
        {
            var d = date.Date;
            return _workingDays.Contains(d.DayOfWeek)
                && !GetCachedHolidays(d.Year).Contains(d);
        }

        /// <inheritdoc/>
        public DateTime AddBusinessDays(DateTime date, int days)
        {
            if (days == 0) 
                return date.Date;

            var current = date.Date;
            int remaining = Math.Abs(days);
            int step = days > 0 ? 1 : -1;

            while (remaining > 0)
            {
                current = current.AddDays(step);
                if (IsBusinessDay(current))
                    remaining--;
            }

            return current;
        }

        /// <inheritdoc/>
        public int BusinessDaysBetween(DateTime start, DateTime end)
        {
            var startDate = start.Date;
            var endDate = end.Date;

            if (startDate == endDate) 
                return 0;

            // Always count forward, apply sign at the end
            bool negative = startDate > endDate;
            if (negative) 
            { 
                var tmp = startDate; 
                startDate = endDate; 
                endDate = tmp; 
            }

            int count = 0;
            var current = startDate;
            while (current < endDate)
            {
                if (IsBusinessDay(current))
                    count++;
                current = current.AddDays(1);
            }

            return negative ? -count : count;
        }

        /// <inheritdoc/>
        public DateTime NextBusinessDay(DateTime date)
        {
            var current = date.Date.AddDays(1);
            while (!IsBusinessDay(current))
                current = current.AddDays(1);
            return current;
        }

        /// <inheritdoc/>
        public DateTime PreviousBusinessDay(DateTime date)
        {
            var current = date.Date.AddDays(-1);
            while (!IsBusinessDay(current))
                current = current.AddDays(-1);
            return current;
        }

        // -------------------------------------------------------------------------
        // Cache
        // -------------------------------------------------------------------------

        private HashSet<DateTime> GetCachedHolidays(int year)
        {
            lock (_cacheLock)
            {
                if (!_holidayCache.TryGetValue(year, out var set))
                {
                    set = new HashSet<DateTime>(_holidayProvider.GetHolidays(year).Select(d => d.Date));
                    _holidayCache[year] = set;
                }
                return set;
            }
        }
    }
}