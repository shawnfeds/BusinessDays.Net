using System;
using System.Collections.Generic;
using BusinessDays.Net.Abstractions;
using BusinessDays.Net.Core;
using FluentAssertions;
using Xunit;

namespace BusinessDays.Net.Tests.Core
{
    public class BusinessCalendarTests
    {
        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private static BusinessCalendar StandardCalendar(IEnumerable<DateTime>? holidays = null)
        {
            if (holidays == null) 
                return new BusinessCalendar();
            return new BusinessCalendar(new FixedHolidayProvider(holidays));
        }

        private static BusinessCalendar SunThuCalendar(IEnumerable<DateTime>? holidays = null)
        {
            var workingDays = new[]
            {
                DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday,
                DayOfWeek.Wednesday, DayOfWeek.Thursday
            };

            if (holidays == null) return new BusinessCalendar(workingDays);
            return new BusinessCalendar(new FixedHolidayProvider(holidays), workingDays);
        }

        // -------------------------------------------------------------------------
        // IsBusinessDay
        // -------------------------------------------------------------------------

        [Fact]
        public void IsBusinessDay_Monday_ReturnsTrue()
        {
            var calendar = StandardCalendar();
            calendar.IsBusinessDay(new DateTime(2024, 1, 8)) // Monday
                .Should().BeTrue();
        }

        [Fact]
        public void IsBusinessDay_Saturday_ReturnsFalse()
        {
            var calendar = StandardCalendar();
            calendar.IsBusinessDay(new DateTime(2024, 1, 6)) // Saturday
                .Should().BeFalse();
        }

        [Fact]
        public void IsBusinessDay_Sunday_ReturnsFalse()
        {
            var calendar = StandardCalendar();
            calendar.IsBusinessDay(new DateTime(2024, 1, 7)) // Sunday
                .Should().BeFalse();
        }

        [Fact]
        public void IsBusinessDay_Holiday_ReturnsFalse()
        {
            var holiday = new DateTime(2024, 1, 8); // Monday but holiday
            var calendar = StandardCalendar(new[] { holiday });
            calendar.IsBusinessDay(holiday).Should().BeFalse();
        }

        [Fact]
        public void IsBusinessDay_IgnoresTimeComponent()
        {
            var calendar = StandardCalendar();
            // Same Monday, different times
            calendar.IsBusinessDay(new DateTime(2024, 1, 8, 0, 0, 0)).Should().BeTrue();
            calendar.IsBusinessDay(new DateTime(2024, 1, 8, 23, 59, 59)).Should().BeTrue();
        }

        [Fact]
        public void IsBusinessDay_HolidayWithTimeComponent_StillRecognisedAsHoliday()
        {
            // Holiday provider returns DateTime with time — must still match
            var holiday = new DateTime(2024, 1, 8, 14, 30, 0);
            var calendar = StandardCalendar(new[] { holiday });
            calendar.IsBusinessDay(new DateTime(2024, 1, 8)).Should().BeFalse();
        }

        [Fact]
        public void IsBusinessDay_SunThuCalendar_SundayIsBusinessDay()
        {
            var calendar = SunThuCalendar();
            calendar.IsBusinessDay(new DateTime(2024, 1, 7)) // Sunday
                .Should().BeTrue();
        }

        [Fact]
        public void IsBusinessDay_SunThuCalendar_FridayIsNotBusinessDay()
        {
            var calendar = SunThuCalendar();
            calendar.IsBusinessDay(new DateTime(2024, 1, 5)) // Friday
                .Should().BeFalse();
        }

        // -------------------------------------------------------------------------
        // AddBusinessDays
        // -------------------------------------------------------------------------

        [Fact]
        public void AddBusinessDays_Zero_ReturnsSameDate()
        {
            var calendar = StandardCalendar();
            var date = new DateTime(2024, 1, 8); // Monday
            calendar.AddBusinessDays(date, 0).Should().Be(date);
        }

        [Fact]
        public void AddBusinessDays_PositiveDays_SkipsWeekends()
        {
            var calendar = StandardCalendar();
            var friday = new DateTime(2024, 1, 5);
            // +1 from Friday should be Monday
            calendar.AddBusinessDays(friday, 1).Should().Be(new DateTime(2024, 1, 8));
        }

        [Fact]
        public void AddBusinessDays_PositiveDays_SkipsHolidays()
        {
            var monday = new DateTime(2024, 1, 8);
            var tuesday = new DateTime(2024, 1, 9); // holiday
            var calendar = StandardCalendar(new[] { tuesday });
            // +1 from Monday skips Tuesday holiday → Wednesday
            calendar.AddBusinessDays(monday, 1).Should().Be(new DateTime(2024, 1, 10));
        }

        [Fact]
        public void AddBusinessDays_NegativeDays_MovesBackward()
        {
            var calendar = StandardCalendar();
            var monday = new DateTime(2024, 1, 8);
            // -1 from Monday = Friday previous week
            calendar.AddBusinessDays(monday, -1).Should().Be(new DateTime(2024, 1, 5));
        }

        [Fact]
        public void AddBusinessDays_AcrossYearBoundary()
        {
            var calendar = StandardCalendar();
            // Dec 31 2024 is Tuesday, +1 = Jan 1 2025 (Wednesday)
            // but Jan 1 is NOT a holiday in NullProvider, so it counts
            var dec31 = new DateTime(2024, 12, 31);
            calendar.AddBusinessDays(dec31, 1).Should().Be(new DateTime(2025, 1, 1));
        }

        [Fact]
        public void AddBusinessDays_AcrossYearBoundary_WithHoliday()
        {
            var newYearsDay = new DateTime(2025, 1, 1);
            var calendar = StandardCalendar(new[] { newYearsDay });
            var dec31 = new DateTime(2024, 12, 31); // Tuesday
            // Jan 1 is holiday, so +1 = Jan 2 (Thursday)
            calendar.AddBusinessDays(dec31, 1).Should().Be(new DateTime(2025, 1, 2));
        }

        [Fact]
        public void AddBusinessDays_LargeNumber_Completes()
        {
            var calendar = StandardCalendar();
            var date = new DateTime(2024, 1, 1);
            // Should not throw or hang
            var result = calendar.AddBusinessDays(date, 260); // ~1 working year
            result.Should().BeAfter(date);
        }

        // -------------------------------------------------------------------------
        // BusinessDaysBetween
        // -------------------------------------------------------------------------

        [Fact]
        public void BusinessDaysBetween_SameDate_ReturnsZero()
        {
            var calendar = StandardCalendar();
            var date = new DateTime(2024, 1, 8);
            calendar.BusinessDaysBetween(date, date).Should().Be(0);
        }

        [Fact]
        public void BusinessDaysBetween_MonToFri_ReturnsFour()
        {
            var calendar = StandardCalendar();
            // Mon to Fri: Mon Tue Wed Thu = 4 (end exclusive)
            calendar.BusinessDaysBetween(
                new DateTime(2024, 1, 8),  // Monday
                new DateTime(2024, 1, 12)) // Friday
                .Should().Be(4);
        }

        [Fact]
        public void BusinessDaysBetween_SpanningWeekend_ExcludesWeekend()
        {
            var calendar = StandardCalendar();
            // Friday to Monday: only Friday counts (end exclusive)
            calendar.BusinessDaysBetween(
                new DateTime(2024, 1, 5),  // Friday
                new DateTime(2024, 1, 8))  // Monday
                .Should().Be(1);
        }

        [Fact]
        public void BusinessDaysBetween_WithHoliday_ExcludesHoliday()
        {
            var wednesday = new DateTime(2024, 1, 10);
            var calendar = StandardCalendar(new[] { wednesday });
            // Mon to Fri with Wed holiday: Mon Tue Thu = 3
            calendar.BusinessDaysBetween(
                new DateTime(2024, 1, 8),
                new DateTime(2024, 1, 12))
                .Should().Be(3);
        }

        [Fact]
        public void BusinessDaysBetween_ReversedDates_ReturnsNegative()
        {
            var calendar = StandardCalendar();
            calendar.BusinessDaysBetween(
                new DateTime(2024, 1, 12), // Friday
                new DateTime(2024, 1, 8))  // Monday
                .Should().Be(-4);
        }

        [Fact]
        public void BusinessDaysBetween_AcrossYearBoundary()
        {
            var calendar = StandardCalendar();
            // Dec 30 2024 (Mon) to Jan 3 2025 (Fri): Mon Tue Wed Thu = 4
            calendar.BusinessDaysBetween(
                new DateTime(2024, 12, 30),
                new DateTime(2025, 1, 3))
                .Should().Be(4);
        }

        // -------------------------------------------------------------------------
        // NextBusinessDay
        // -------------------------------------------------------------------------

        [Fact]
        public void NextBusinessDay_FromFriday_ReturnsMonday()
        {
            var calendar = StandardCalendar();
            calendar.NextBusinessDay(new DateTime(2024, 1, 5)) // Friday
                .Should().Be(new DateTime(2024, 1, 8));        // Monday
        }

        [Fact]
        public void NextBusinessDay_IsStrictlyAfter_GivenDate()
        {
            var calendar = StandardCalendar();
            var monday = new DateTime(2024, 1, 8);
            // Even if Monday is a business day, Next must be strictly after
            calendar.NextBusinessDay(monday).Should().Be(new DateTime(2024, 1, 9));
        }

        [Fact]
        public void NextBusinessDay_SkipsHoliday()
        {
            var tuesday = new DateTime(2024, 1, 9);
            var calendar = StandardCalendar(new[] { tuesday });
            calendar.NextBusinessDay(new DateTime(2024, 1, 8)) // Monday
                .Should().Be(new DateTime(2024, 1, 10));        // Wednesday
        }

        // -------------------------------------------------------------------------
        // PreviousBusinessDay
        // -------------------------------------------------------------------------

        [Fact]
        public void PreviousBusinessDay_FromMonday_ReturnsFriday()
        {
            var calendar = StandardCalendar();
            calendar.PreviousBusinessDay(new DateTime(2024, 1, 8)) // Monday
                .Should().Be(new DateTime(2024, 1, 5));             // Friday
        }

        [Fact]
        public void PreviousBusinessDay_IsStrictlyBefore_GivenDate()
        {
            var calendar = StandardCalendar();
            var friday = new DateTime(2024, 1, 5);
            calendar.PreviousBusinessDay(friday).Should().Be(new DateTime(2024, 1, 4));
        }

        [Fact]
        public void PreviousBusinessDay_SkipsHoliday()
        {
            var friday = new DateTime(2024, 1, 5);
            var calendar = StandardCalendar(new[] { friday });
            calendar.PreviousBusinessDay(new DateTime(2024, 1, 8)) // Monday
                .Should().Be(new DateTime(2024, 1, 4));             // Thursday
        }

        // -------------------------------------------------------------------------
        // Constructor validation
        // -------------------------------------------------------------------------

        [Fact]
        public void Constructor_NullHolidayProvider_Throws()
        {
            Action act = () => new BusinessCalendar(null!, new[] { DayOfWeek.Monday });
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_EmptyWorkingDays_Throws()
        {
            Action act = () => new BusinessCalendar(Array.Empty<DayOfWeek>());
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_NullWorkingDays_Throws()
        {
            Action act = () => new BusinessCalendar((DayOfWeek[])null!);
            act.Should().Throw<ArgumentException>();
        }
    }
}