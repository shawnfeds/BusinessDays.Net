using System;
using System.Collections.Generic;

namespace BusinessDays.Net.Abstractions
{
    /// <summary>
    /// Provides holidays for a given year.
    /// Implement this to plug in any holiday source — static lists, Nager.Date, a database, etc.
    /// </summary>
    public interface IHolidayProvider
    {
        /// <summary>
        /// Returns all holidays for the given year.
        /// Time components are ignored — only the date portion is used.
        /// </summary>
        IEnumerable<DateTime> GetHolidays(int year);
    }
}