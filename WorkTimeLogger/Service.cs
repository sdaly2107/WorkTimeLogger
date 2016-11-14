using Microsoft.Win32;
using NLog;
using System;
using System.IO;
using Topshelf;
using WorkTimeLogger.Extensions;
using WorkTimeLogger.Interfaces;
using System.Threading;
using System.Globalization;
using WorkTimeLogger.Extensions;

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

        private void ProcessTimeData()
        {
            ICalculator hoursCalculator = new HoursCalclulator(new Settings());
            var data = hoursCalculator.ProcessWeek(DateTime.Now);
            var formatter = new HoursFormatter();
            string output = formatter.Format(data);

            output += $"{Environment.NewLine}{Environment.NewLine}Hours worked in week: {hoursCalculator.HoursWorked.ToString("#.##")} ({hoursCalculator.HoursToWork.ToReadableTime()})";
            output += $"{Environment.NewLine}Hours to work: {hoursCalculator.HoursToWork.ToString("#.##")} ({hoursCalculator.HoursToWork.ToReadableTime()})";

            int week = DateTime.Now.WeekNumber();
            string outputpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "WorkTimeLogger", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), "Hours for week " + week + ".txt");

            File.WriteAllText(outputpath, output);
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
                ProcessTimeData();
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to update hours summary - ", ex.Message);
            }
        }

        public void Start()
        {
            _logger.Info("Service starting");
            SetDefaultCulture();
            ProcessTimeData();
        }

        public void Stop()
        {
            _logger.Info("Service stopping");
        }
    }
}
