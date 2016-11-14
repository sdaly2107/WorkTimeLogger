using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Moq;
using System;
using WorkTimeLogger;
using WorkTimeLogger.Interfaces;
using WorkTimeLogger.Models;
using System.Collections.Generic;
using FluentAssertions;
using System.Linq;

namespace WorkTimeLoggerTests
{
    [TestClass]
    public class HoursCalculatorTests
    {
        private HoursCalclulator _subject;

        private Mock<IDataProvider> _mockDataProvider;

        [TestInitializeAttribute]
        public void SetUp()
        {
            var settings = new Settings
            {
                HoursBeforeLunchDeducted = 5,
                Lunch = 0.5,
                NoShowHours = 7.4,
                MinWeekHours = 37,
                BandwidthHours = new HourRange(7, 19)
            };

            _mockDataProvider = new Mock<IDataProvider>();

            _subject = new HoursCalclulator(settings, _mockDataProvider.Object);
        }

        [TestMethod]
        public void TestProcessWeek_DocksLunch()
        {
            var eventstart = new WorkEvent
            {
                Time = new DateTime(2016, 1, 1, 8, 0, 0),
                Type = SessionSwitchReason.SessionUnlock
            };

            var eventend = new WorkEvent
            {
                Time = new DateTime(2016, 1, 1, 16, 00, 0),
                Type = SessionSwitchReason.SessionLock
            };

            var data = new List<WorkDay>()
            {
                new WorkDay
                {
                    Date = DateTime.Now,
                    Events = new List<WorkEvent> { eventstart, eventend }
                }

            };

            _mockDataProvider.Setup(x => x.GetWeekEvents(It.IsAny<DateTime>())).Returns(data);

            IList<Hours> result = _subject.ProcessWeek(DateTime.Now);

            result.Count.Should().Be(1, "1 day worked");
            result.First().HoursWorked.Should().Be(7.5);
        }

        [TestMethod]
        public void TestProcessWeek_DoesNOTDockLunchWhenLittleHoursWorked()
        {
            var eventstart = new WorkEvent
            {
                Time = new DateTime(2016, 1, 1, 9, 0, 0),
                Type = SessionSwitchReason.SessionUnlock
            };

            var eventend = new WorkEvent
            {
                Time = new DateTime(2016, 1, 1, 12, 00, 0),
                Type = SessionSwitchReason.SessionLock
            };

            var data = new List<WorkDay>()
            {
                new WorkDay
                {
                    Date = DateTime.Now,
                    Events = new List<WorkEvent> { eventstart, eventend }
                }

            };

            _mockDataProvider.Setup(x => x.GetWeekEvents(It.IsAny<DateTime>())).Returns(data);

            IList<Hours> result = _subject.ProcessWeek(DateTime.Now);

            result.First().HoursWorked.Should().Be(3);
        }

        [TestMethod]
        public void TestProcessWeek_IncrementsHoursWorked()
        {
            var eventstart = new WorkEvent
            {
                Time = new DateTime(2016, 1, 1, 9, 0, 0),
                Type = SessionSwitchReason.SessionUnlock
            };

            var eventend = new WorkEvent
            {
                Time = new DateTime(2016, 1, 1, 12, 00, 0),
                Type = SessionSwitchReason.SessionLock
            };

            //6 hours over two days
            var data = new List<WorkDay>()
            {
                new WorkDay
                {
                    Date = DateTime.Now,
                    Events = new List<WorkEvent> { eventstart, eventend }
                },
                 new WorkDay
                {
                    Date = DateTime.Now,
                    Events = new List<WorkEvent> { eventstart, eventend }
                }

            };

            _mockDataProvider.Setup(x => x.GetWeekEvents(It.IsAny<DateTime>())).Returns(data);

            _subject.ProcessWeek(DateTime.Now);

            _subject.HoursWorked.Should().Be(6);
            _subject.HoursToWork.Should().Be(31);
        }


        [TestMethod]
        public void TestProcessWeek_HoursOutSideBandwidthHoursNOTcounted()
        {
            var eventstart = new WorkEvent
            {
                Time = new DateTime(2016, 1, 1, 6, 0, 0),
                Type = SessionSwitchReason.SessionUnlock
            };

            var eventend = new WorkEvent
            {
                Time = new DateTime(2016, 1, 1, 20, 00, 0),
                Type = SessionSwitchReason.SessionLock
            };

            //6am - 8pm
            var data = new List<WorkDay>()
            {
                new WorkDay
                {
                    Date = DateTime.Now,
                    Events = new List<WorkEvent> { eventstart, eventend }
                }

            };

            _mockDataProvider.Setup(x => x.GetWeekEvents(It.IsAny<DateTime>())).Returns(data);

            IList<Hours> result = _subject.ProcessWeek(DateTime.Now);

            //hour docked from start and end time, 30 mins docked for lunch
            result.First().HoursWorked.Should().Be(11.5);
        }

        [TestMethod]
        public void TestProcessWeek_CalculatesHoursWhenNoLockEvent()
        {
            //freeze now time to 10am
            var frozenTime = new DateTime(2016, 1, 1, 10, 0, 0);
            _subject.DateNow = frozenTime;

            //9am
            var eventstart = new WorkEvent
            {
                Time = new DateTime(2016, 1, 1, 9, 0, 0),
                Type = SessionSwitchReason.SessionUnlock
            };

            //day started but not ended yet
            var data = new List<WorkDay>()
            {
                new WorkDay
                {
                    Date = DateTime.Now,
                    Events = new List<WorkEvent> { eventstart }
                }
            };

            _mockDataProvider.Setup(x => x.GetWeekEvents(It.IsAny<DateTime>())).Returns(data);

            IList<Hours> result = _subject.ProcessWeek(DateTime.Now);

            result.First().HoursWorked.Should().Be(1);
        }

    }
}
