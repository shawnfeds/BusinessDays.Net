using System;
using System.Collections.Generic;
using BusinessDays.Net.Abstractions;

namespace BusinessDays.Net.Core
{
    /// <summary>
    /// Default provider — no holidays. 
    /// Used when caller doesn't supply a provider.
    /// </summary>
    ///
    public sealed class NullHolidayProvider : IHolidayProvider
    {
        /// <inheritdoc/>
        public static readonly NullHolidayProvider Instance = new NullHolidayProvider();

        private NullHolidayProvider() { }

        /// <inheritdoc/>
        public IEnumerable<DateTime> GetHolidays(int year)
        {
            return Array.Empty<DateTime>();
        }
    }
}