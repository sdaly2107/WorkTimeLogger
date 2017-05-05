using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeLogger.Models;

namespace WorkTimeLogger.Interfaces
{
    public interface ICalculator
    {
        double HoursWorked { get; set; }

        double HoursToWork { get; set; }

        DateTime FridayStartTime { get; set; }

        double HoursToWorkExcludingFriday { get; set; }

        IList<Hours> ProcessWeek(DateTime time);

    }
}
