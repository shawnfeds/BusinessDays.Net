using System;

namespace BusinessDays.Net.Abstractions
{
    /// <summary>
    /// Core business calendar operations.
    /// All DateTime parameters are treated as date-only — time components are ignored.
    /// </summary>
    public interface IBusinessCalendar
    {
        /// <summary>
        /// Returns true if the given date is a working day
        /// (not a weekend, not a holiday).
        /// </summary>
        bool IsBusinessDay(DateTime date);

        /// <summary>
        /// Adds the given number of business days to a date.
        /// Negative values move backwards.
        /// If date itself is not a business day, counting starts
        /// from the next (or previous) business day.
        /// </summary>
        DateTime AddBusinessDays(DateTime date, int days);

        /// <summary>
        /// Returns the count of business days between start and end (exclusive of end).
        /// Returns 0 if start == end or there are no business days in range.
        /// </summary>
        int BusinessDaysBetween(DateTime start, DateTime end);

        /// <summary>
        /// Returns the next business day strictly after the given date.
        /// </summary>
        DateTime NextBusinessDay(DateTime date);

        /// <summary>
        /// Returns the most recent business day strictly before the given date.
        /// </summary>
        DateTime PreviousBusinessDay(DateTime date);
    }
}