using System;
using System.Collections.Generic;
using System.Text;
using WorkTimeLogger.Models;

namespace WorkTimeLogger
{
    public class HoursFormatter
    {
        private string ToReadableTime(double hours)
        {
            StringBuilder sb = new StringBuilder();
            TimeSpan hoursTS = TimeSpan.FromHours(hours);

            if ((int)hoursTS.TotalHours > 0)
            {
                sb.Append((int)hoursTS.TotalHours).Append(" hours and ");
            }

            sb.Append(hoursTS.Minutes).Append(" minutes");

            return sb.ToString();
        }

        public string Format(IList<Hours> hours)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Hours day in hours)
            {
                sb.Append(day.Date.DayOfWeek).Append(Environment.NewLine);
                sb.Append(new string('-', 10)).Append(Environment.NewLine);

                if (day.StartTime == default(DateTime))
                {
                    sb.Append("Holiday / Sick / Out of office").Append(Environment.NewLine);
                }
                else
                {
                    sb.Append("Start: ").Append(day.StartTime).Append(Environment.NewLine);
                    sb.Append("End: ").Append(day.EndTime).Append(Environment.NewLine);
                }

                sb.Append(Environment.NewLine);

                sb.Append("Hours worked: ").Append(day.HoursWorked.ToString("#.##"));
                sb.Append(" (").Append(ToReadableTime(day.HoursWorked)).Append(")");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
            }

            sb.Append(new string('-', 20));

            return sb.ToString();
        }
    }
}
