using System;
using BusinessDays.Net.Abstractions;

namespace BusinessDays.Net.Core
{
    /// <inheritdoc/>
    public static class BusinessCalendarExtensions
    {
        /// <summary>
        /// Returns the nth business day of the given month.
        /// e.g. n=1 returns the first business day of the month.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if n exceeds the number of business days in the month.
        /// </exception>
        public static DateTime NthBusinessDayOfMonth(this IBusinessCalendar calendar, int year, int month, int n)
        {
            if (n <= 0) 
                throw new ArgumentOutOfRangeException(nameof(n), "n must be at least 1.");

            var current = new DateTime(year, month, 1);
            int found = 0;

            while (current.Month == month)
            {
                if (calendar.IsBusinessDay(current))
                {
                    found++;
                    if (found == n) 
                        return current;
                }
                current = current.AddDays(1);
            }

            throw new ArgumentOutOfRangeException(nameof(n), $"Month {year}-{month:D2} does not have {n} business days.");
        }

        /// <summary>
        /// Returns the nth occurrence of a specific weekday in the month
        /// that is also a business day.
        /// e.g. nth=2, day=Friday returns the 2nd Friday that isn't a holiday.
        /// </summary>
        public static DateTime NthBusinessWeekdayOfMonth(this IBusinessCalendar calendar, int year, int month, int n,
            DayOfWeek day)
        {
            if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n), "n must be at least 1.");

            var current = new DateTime(year, month, 1);
            int found = 0;

            while (current.Month == month)
            {
                if (current.DayOfWeek == day && calendar.IsBusinessDay(current))
                {
                    found++;
                    if (found == n) 
                        return current;
                }
                current = current.AddDays(1);
            }

            throw new ArgumentOutOfRangeException(nameof(n),
                $"Month {year}-{month:D2} does not have {n} business {day}s.");
        }
    }
}