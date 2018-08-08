using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public static class DateTimeExtensions
    {
        public static int CalculateCalendarQuarter(this DateTime value)
        {
            return (int)Math.Floor(((decimal)value.Month - 1) / 3) + 1;
        }

        public static string ToCalendarQuarterString(this DateTime value)
        {
            int quarter = CalculateCalendarQuarter(value);
            return "Q" + quarter;
        }

        public static DateTime CalculateCalendarQuarterStart(this DateTime value)
        {
            int quarter = CalculateCalendarQuarter(value);
            int month = 3 * quarter - 2;
            return new DateTime(value.Year, month, 1);
        }

        public static DateTime CalculateCalendarQuarterEnd(this DateTime value)
        {
            int quarter = CalculateCalendarQuarter(value);
            if (value.Year == 9999 && quarter == 4)
            {
                return new DateTime(9999, 12, 31);
            }
            return new DateTime(value.Year, 3 * quarter, 1).AddMonths(1).AddDays(-1);
        }
    }
}