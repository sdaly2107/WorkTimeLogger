using Microsoft.Win32;
using NLog;
using System;
using WorkTimeLogger.Interfaces;
using WorkTimeLogger.Models;

namespace WorkTimeLogger
{
    /// <summary>
    /// Handles session switch events
    /// </summary>
    public class SessionLogger : ISessionLogger
    {
        private readonly IDataProvider _dataProvider;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public SessionLogger()
        {
            _dataProvider = new XMLFileDataProvider();
        }

        public SessionLogger(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        private bool ShouldIgnoreEvent(SessionSwitchReason reason)
        {
            return reason != SessionSwitchReason.SessionLock &&
                   reason != SessionSwitchReason.SessionUnlock &&
                   reason != SessionSwitchReason.SessionLogon &&
                   reason != SessionSwitchReason.SessionLogoff;
        }

        public void Log(SessionSwitchReason reason)
        {
            if (ShouldIgnoreEvent(reason))
            {
                _logger.Debug($"Uninteresting event {reason} received");
                return;
            }

            DateTime eventtime = DateTime.Now; //local time
            var newevent = new WorkEvent
            {
                Type = reason,
                Time = eventtime
            };

            _dataProvider.UpdateEvents(eventtime, newevent);
            _logger.Info($"{reason} event logged");
        }

    }

}

