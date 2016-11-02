using System;
using WorkTimeLogger.Models;
using System.Collections.Generic;

namespace WorkTimeLogger.Interfaces
{
    public interface IDataProvider
    {
        void UpdateEvents(DateTime time, WorkEvent newevent);

        IEnumerable<WorkDay> GetWeekEvents(DateTime time);
    }
}
