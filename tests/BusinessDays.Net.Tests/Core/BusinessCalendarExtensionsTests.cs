using System;
using BusinessDays.Net.Core;
using FluentAssertions;
using Xunit;

namespace BusinessDays.Net.Tests.Core
{
    public class BusinessCalendarExtensionsTests
    {
        private static BusinessCalendar StandardCalendar() => new BusinessCalendar();

        // -------------------------------------------------------------------------
        // NthBusinessDayOfMonth
        // -------------------------------------------------------------------------

        [Fact]
        public void NthBusinessDayOfMonth_First_ReturnsFirstBusinessDay()
        {
            var calendar = StandardCalendar();
            // Jan 1 2024 is Monday
            calendar.NthBusinessDayOfMonth(2024, 1, 1).Should().Be(new DateTime(2024, 1, 1));
        }

        [Fact]
        public void NthBusinessDayOfMonth_WhenFirstIsWeekend_SkipsToMonday()
        {
            var calendar = StandardCalendar();
            // June 1 2024 is Saturday — first business day is Monday June 3
            calendar.NthBusinessDayOfMonth(2024, 6, 1).Should().Be(new DateTime(2024, 6, 3));
        }

        [Fact]
        public void NthBusinessDayOfMonth_ExceedsAvailable_Throws()
        {
            var calendar = StandardCalendar();
            Action act = () => calendar.NthBusinessDayOfMonth(2024, 1, 99);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NthBusinessDayOfMonth_ZeroN_Throws()
        {
            var calendar = StandardCalendar();
            Action act = () => calendar.NthBusinessDayOfMonth(2024, 1, 0);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        // -------------------------------------------------------------------------
        // NthBusinessWeekdayOfMonth
        // -------------------------------------------------------------------------

        [Fact]
        public void NthBusinessWeekdayOfMonth_SecondFriday_ReturnsCorrectDate()
        {
            var calendar = StandardCalendar();
            // Second Friday of Jan 2024 = Jan 12
            calendar.NthBusinessWeekdayOfMonth(2024, 1, 2, DayOfWeek.Friday).Should().Be(new DateTime(2024, 1, 12));
        }

        [Fact]
        public void NthBusinessWeekdayOfMonth_WhenWeekdayIsHoliday_SkipsIt()
        {
            // Jan 12 2024 (second Friday) is a holiday
            var holiday = new DateTime(2024, 1, 12);
            var calendar = new BusinessCalendar(new FixedHolidayProvider(new[] { holiday }));
            // Second business Friday should now be Jan 19
            calendar.NthBusinessWeekdayOfMonth(2024, 1, 2, DayOfWeek.Friday).Should().Be(new DateTime(2024, 1, 19));
        }

        [Fact]
        public void NthBusinessWeekdayOfMonth_ExceedsAvailable_Throws()
        {
            var calendar = StandardCalendar();
            Action act = () => calendar.NthBusinessWeekdayOfMonth(2024, 1, 6, DayOfWeek.Friday);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}