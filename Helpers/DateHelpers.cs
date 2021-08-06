using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hotvenues.Helpers
{
    public class DateHelpers
    {
        public static int GetAge(DateTime birthdate)
        {
            // Save today's date.
            var today = DateTime.Today;
            // Calculate the age.
            var age = today.Year - birthdate.Year;
            // Go back to the year the person was born in case of a leap year
            if (birthdate > today.AddYears(-age)) age--;

            return age;
        }

        public static IEnumerable<DateTime> PeriodMonths(DateTime d0, DateTime d1)
        {
            return Enumerable.Range(0, (d1.Year - d0.Year) * 12 + (d1.Month - d0.Month + 1))
                             .Select(m => new DateTime(d0.Year, d0.Month, 1).AddMonths(m));
        }

        public static string GetFullAge(DateTime? date)
        {
            if(!date.HasValue) return "";
            var dob = date.Value;
            if (dob > DateTime.UtcNow) return "";
            var now = DateTime.Now;
            var years = new DateTime(DateTime.Now.Subtract(dob).Ticks).Year - 1;
            var pastYearDate = dob.AddYears(years);
            var months = 0;
            for (var i = 1; i <= 12; i++)
            {
                if (pastYearDate.AddMonths(i) == now)
                {
                    months = i;
                    break;
                }
                if (pastYearDate.AddMonths(i) < now) continue;
                months = i - 1;
                break;
            }
            var days = now.Subtract(pastYearDate.AddMonths(months)).Days;
            return $"{(years > 0 ? "Years".ToQuantity(years) : "")} {(months > 0 ? "Month".ToQuantity(months) : "")} {(days > 0 ? "Day".ToQuantity(days) : "")}".Trim();
        }


        public static long GetAgeDays(DateTime dob)
        {
            return dob > DateTime.UtcNow ? 0 : (DateTime.UtcNow - dob).Days;
        }


        public static string GetFullDuration(DateTime endDate, DateTime? startDate)
        {
            var now = startDate ?? DateTime.Now;
            var then = endDate;
            var years = new DateTime(then.Subtract(now).Ticks).Year - 1;
            var pastYearDate = now.AddYears(years);
            var months = 0;
            for (var i = 1; i <= 12; i++)
            {
                if (pastYearDate.AddMonths(i) == then)
                {
                    months = i;
                    break;
                }
                if (pastYearDate.AddMonths(i) < then) continue;
                months = i - 1;
                break;
            }
            var days = then.Subtract(pastYearDate.AddMonths(months)).Days;
            return $"{(years > 0 ? "Years".ToQuantity(years) : "")} {(months > 0 ? "Month".ToQuantity(months) : "")} {(days > 0 ? "Day".ToQuantity(days) : "")}".Trim();
        }

        /// <summary>
        /// Gets the date range.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <returns></returns>
        public static DateRange GetDateRange(DatePeriod period)
        {
            switch (period)
            {
                case DatePeriod.Last7Days:
                    return new DateRange(DateTime.UtcNow, -7);
                case DatePeriod.Today:
                    return new DateRange(DateTime.UtcNow);
                case DatePeriod.Week:
                    var s = DateTime.UtcNow.AddDays((DaySub(DateTime.UtcNow.DayOfWeek)));
                    return new DateRange(s, s.AddDays(6));
                case DatePeriod.Month:
                    var x = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    return new DateRange(x, new DateTime(x.Year, x.Month, DateTime.DaysInMonth(x.Year, x.Month)));
                case DatePeriod.Year:
                    return new DateRange(new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 12, 31));
                default:
                    return new DateRange(new DateTime(2010, 1, 1), DateTime.UtcNow);
            }
        }

        public static DateRange GetDateRange(DatePeriod period, DateTime date)
        {
            switch (period)
            {
                case DatePeriod.Last7Days:
                    return new DateRange(date, -7);
                case DatePeriod.Today:
                    return new DateRange(date);
                case DatePeriod.Week:
                    var s = date.AddDays((DaySub(date.DayOfWeek)));
                    return new DateRange(s, s.AddDays(6));
                case DatePeriod.Month:
                    var x = new DateTime(date.Year, date.Month, 1);
                    return new DateRange(x, new DateTime(x.Year, x.Month, DateTime.DaysInMonth(x.Year, x.Month)));
                case DatePeriod.Year:
                    return new DateRange(new DateTime(date.Year, 1, 1), new DateTime(date.Year, 12, 31));
                default:
                    return new DateRange(date, DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Find number of days to subtract from current day.
        /// </summary>
        /// <param name="dayName">Name of the day.</param>
        /// <returns></returns>
        private static int DaySub(DayOfWeek dayName)
        {
            switch (dayName)
            {
                case DayOfWeek.Sunday:
                    return 0;
                case DayOfWeek.Monday:
                    return -1;
                case DayOfWeek.Tuesday:
                    return -2;
                case DayOfWeek.Wednesday:
                    return -3;
                case DayOfWeek.Thursday:
                    return -4;
                case DayOfWeek.Friday:
                    return -5;
                case DayOfWeek.Saturday:
                    return -6;
                default:
                    return 0;
            }
        }
    }

    public class DateRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public DateRange(DateTime startDate, DateTime endDate)
        {
            Start = startDate;
            End = endDate.AddHours(23);
        }

        public DateRange(DateTime date)
        {
            Start = date;
            End = date.AddHours(23);
        }

        public DateRange(DateTime date, int days)
        {
            Start = date.AddDays(days);
            End = date;
        }
    }

    public enum DatePeriod
    {
        Today,
        Week,
        Month,
        Year,
        Last7Days
    }
}
