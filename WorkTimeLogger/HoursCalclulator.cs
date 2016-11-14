using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkTimeLogger.Interfaces;
using WorkTimeLogger.Models;

namespace WorkTimeLogger
{
    public class HoursCalclulator : ICalculator
    {
        private readonly IDataProvider _dataProvider;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly Settings _settings;

        public double HoursWorked { get; set; }

        public double HoursToWork { get; set; }

        private DateTime _datetimeNow;
        public DateTime DateNow
        {
            get
            {
                if(default(DateTime) != _datetimeNow)
                {
                    return _datetimeNow;
                }

                return DateTime.Now;
            }

            set
            {
                _datetimeNow = value;
            }

        }


        public HoursCalclulator(Settings settings)
        {
            _settings = settings;
            _dataProvider = new XMLFileDataProvider();
        }

        public HoursCalclulator(Settings settings, IDataProvider dataProvider)
        {
            _settings = settings;
            _dataProvider = dataProvider;
        }

        public IList<Hours> ProcessWeek(DateTime time)
        {
            IList<Hours> hours = new List<Hours>();

            HoursToWork = _settings.MinWeekHours;

            var workdays = _dataProvider.GetWeekEvents(time);

            foreach (WorkDay workday in workdays)
            {

                Hours currentDayHours = new Hours
                {
                    Date = workday.Date
                };

                var startEvents = workday.Events.Where(x => x.Type == SessionSwitchReason.SessionLogon || x.Type == SessionSwitchReason.SessionUnlock)
                                                     .Select(x => x.Time);

                var endEvents = workday.Events.Where(x => x.Type == SessionSwitchReason.SessionLogoff || x.Type == SessionSwitchReason.SessionLock)
                                                    .Select(x => x.Time);


                currentDayHours.HoursWorked = _settings.NoShowHours;

                if (!endEvents.Any() && !startEvents.Any())
                {
                    if (workday.Date.Date >= DateTime.Now.Date)
                    {
                        _logger.Debug($"No events for {currentDayHours.Date.ToString()}, but day is in future");
                        continue;
                    }
                }
                else
                {
                    DateTime startTime = startEvents.Min();

                    //last lock event, or now if no lock
                    DateTime endTime = endEvents.Any() ? endEvents.Max() : this.DateNow;

                    currentDayHours.StartTime = startTime;
                    currentDayHours.EndTime = endTime;

                    TrimToBandwidthHours(ref startTime, ref endTime);
                    TimeSpan workSpan = endTime - startTime;
                    currentDayHours.HoursWorked = workSpan.TotalHours;

                    DockLunch(currentDayHours);
                }

                hours.Add(currentDayHours);
                UpdateHoursWorkedForWeek(currentDayHours);
            }

            return hours;
        }

        private void TrimToBandwidthHours(ref DateTime startTime, ref DateTime endTime)
        {
            if (startTime.Hour < _settings.BandwidthHours.from)
            {
                startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, _settings.BandwidthHours.from, 0, 0);
            }

            if (endTime.Hour > _settings.BandwidthHours.to)
            {
                endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, _settings.BandwidthHours.to, 0, 0);
            }
        }

        private void DockLunch(Hours currentDayHours)
        {
            if (currentDayHours.HoursWorked > _settings.HoursBeforeLunchDeducted)
            {
                currentDayHours.HoursWorked -= _settings.Lunch;
            }
        }

        private void UpdateHoursWorkedForWeek(Hours currentDayHours)
        {
            HoursWorked += currentDayHours.HoursWorked;
            HoursToWork = _settings.MinWeekHours - HoursWorked;
        }

    }
}
