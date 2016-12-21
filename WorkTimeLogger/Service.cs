using Microsoft.Win32;
using NLog;
using System;
using System.IO;
using Topshelf;
using WorkTimeLogger.Extensions;
using WorkTimeLogger.Interfaces;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;

namespace WorkTimeLogger
{
    public class Service
    {
        private readonly ISessionLogger _sessionlogger;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public Service()
        {
            _sessionlogger = new SessionLogger();
        }

        public Service(ISessionLogger sessionLogger, ICalculator hoursCalculator)
        {
            _sessionlogger = sessionLogger;
        }

        private void SetDefaultCulture()
        {
            _logger.Info("Current Culture: {0}", Thread.CurrentThread.CurrentCulture);

            string gbCultureName = "en-GB";
            if (Thread.CurrentThread.CurrentCulture.Name == gbCultureName)
            {
                return;
            }

            var gbCulture = CultureInfo.CreateSpecificCulture(gbCultureName);
            CultureInfo.DefaultThreadCurrentCulture = gbCulture;
            CultureInfo.DefaultThreadCurrentUICulture = gbCulture;

            _logger.Info("Culture updated");
        }

        private void ProcessTimeDataForWeek()
        {
            ProcessTimeData(DateTime.Now);
        }

        private void ProcessTimeData(DateTime time)
        {
            var settings = new Settings();
            ICalculator hoursCalculator = new HoursCalclulator(settings);
            var data = hoursCalculator.ProcessWeek(time);
            var formatter = new HoursFormatter();
            string output = formatter.Format(data, settings);

            string hoursToWorkLabel = hoursCalculator.HoursToWork == 0 ? "No hours to work"
                                                                       : hoursCalculator.HoursToWork > 0 ? "Hours to work" : "Hours cumulated";


            output += $"{Environment.NewLine}{Environment.NewLine}Hours worked in week: {hoursCalculator.HoursWorked.ToString("#.##")} ({hoursCalculator.HoursWorked.ToReadableTime()})";
            output += $"{Environment.NewLine}{hoursToWorkLabel}: {hoursCalculator.HoursToWork.ToString("#.##")} ({hoursCalculator.HoursToWork.ToReadableTime()})";

            int week = time.WeekNumber();
            string basepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WorkTimeLogger");

            //update file with summary for current week
            string hoursForCurrentWeekPath = Path.Combine(basepath, "Hours for current week.txt");
            File.WriteAllText(hoursForCurrentWeekPath, output);

            //write copy of file for history
            string hoursForWeekSummaryPath = Path.Combine(basepath, time.Year.ToString(), time.Month.ToString(), "Hours for week " + week + ".txt");
            File.WriteAllText(hoursForWeekSummaryPath, output);
        }

        public void SessionChange(SessionChangedArguments e)
        {
            _logger.Info("Session change fired");
            SetDefaultCulture();

            try
            {
                _sessionlogger.Log((SessionSwitchReason)e.ReasonCode);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to log event - ", ex.Message);
            }

            try
            {
                ProcessTimeDataForWeek();
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to update hours summary - ", ex.Message);
            }
        }

        private void ScheduleMidnightProcessing()
        {
            //if user is on holiday for the week and their pc is left on then this will allow the week data summary (stating hol/out of office) to be written still 
            DateTime now = DateTime.Now;
            DateTime tomorrow = now.AddDays(1).Date.AddMinutes(5); //five past midnight

            Task.Delay(tomorrow - now).ContinueWith(_ =>
            {
                ProcessTimeDataForWeek();
                ScheduleMidnightProcessing(); //reschedule again for the following day
            });
        }

        public void Start()
        {
            _logger.Info("Service starting");
            SetDefaultCulture();

            //process last weeks data incase user was holiday and their machine was off
            DateTime lastweek = DateTime.Now.AddDays(-7);
            ProcessTimeData(lastweek);

            ProcessTimeDataForWeek();
            ScheduleMidnightProcessing();
        }

        public void Stop()
        {
            _logger.Info("Service stopping");
        }
    }
}
