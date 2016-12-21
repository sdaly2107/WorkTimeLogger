using System;
using System.Text;

namespace WorkTimeLogger.Extensions
{
    public static class DoubleExtensions
    {
        public static string ToReadableTime(this double hours)
        {
            StringBuilder sb = new StringBuilder();
            TimeSpan hoursTS = TimeSpan.FromHours(hours);

            int totalhours = (int)hoursTS.TotalHours;
            string hourPostfix = totalhours == 1 ? " hour" : " hours";
            string minsPostfix = hoursTS.Minutes == 1 ? " minute" : " minutes";

            sb.Append(totalhours).Append(hourPostfix);
            sb.Append(" and ");
            sb.Append(hoursTS.Minutes).Append(minsPostfix);

            return sb.ToString();
        }
    }
}
