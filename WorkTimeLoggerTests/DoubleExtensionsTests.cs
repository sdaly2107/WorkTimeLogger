using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeLogger.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace WorkTimeLoggerTests
{
    [TestClass]
    public class DoubleExtensionsTests
    {
        [TestMethod]
        public void TestToReadableTime_ConvertsHoursToStringOK()
        {
            double hours = 1;

            hours.ToReadableTime().Should().Be("1 hour");

        }

        [TestMethod]
        public void TestToReadableTime_WithMinsToStringOK()
        {
            double hours = 7.4;

            hours.ToReadableTime().Should().Be("7 hours and 24 minutes");
        }

        [TestMethod]
        public void TestToReadableTime_JustMinsToStringOK()
        {
            double hours = 0.4;

            hours.ToReadableTime().Should().Be("24 minutes");
        }

    }
}
