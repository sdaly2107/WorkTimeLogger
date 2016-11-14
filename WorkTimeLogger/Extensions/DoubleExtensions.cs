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

            if(totalhours == 1)
            {
                sb.Append(totalhours).Append(" hour");

            }else if (totalhours > 0)
            {
                sb.Append(totalhours).Append(" hours");
            }

            if(hoursTS.Minutes > 0)
            {
                if(totalhours > 0) sb.Append(" and ");

                sb.Append(hoursTS.Minutes).Append(" minutes");
            }
            

            return sb.ToString();
        }
    }
}
