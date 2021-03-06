﻿using System;
using System.Globalization;
using System.Threading;

namespace WorkTimeLogger.Extensions
{
    public static class DateTimeExtensions
    {
        public static int WeekNumber(this DateTime date)
        {
            DateTimeFormatInfo formatinfo = DateTimeFormatInfo.CurrentInfo;
            int weeknumber = formatinfo.Calendar.GetWeekOfYear(date, formatinfo.CalendarWeekRule, formatinfo.FirstDayOfWeek);

            return weeknumber;
        }

        public static DateTime FirstDateOfWeek(this DateTime date)
        {
            int offset = date.DayOfWeek - DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;
            var firstDate = date.AddDays(-offset).Date;
            
            return firstDate;
        }


    }
}
