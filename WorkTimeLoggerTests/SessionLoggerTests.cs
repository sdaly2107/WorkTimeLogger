using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Moq;
using System;
using WorkTimeLogger;
using WorkTimeLogger.Interfaces;
using WorkTimeLogger.Models;

namespace WorkTimeLoggerTests
{
    [TestClass]
    public class SessionLoggerTests
    {
        private SessionLogger _subject;
        private Mock<IDataProvider> _mockDataProvider;

        [TestInitializeAttribute]
        public void SetUp()
        {
            _mockDataProvider = new Mock<IDataProvider>();

            _subject = new SessionLogger(_mockDataProvider.Object);
        }

        [TestMethod]
        public void TestLog_LogsLockEvent()
        {

            _subject.Log(SessionSwitchReason.SessionLock);

            _mockDataProvider.Verify(x => x.UpdateEvents(It.IsAny<DateTime>(), It.IsAny<WorkEvent>()));
        }

        [TestMethod]
        public void TestLog_LogsUnlockEvent()
        {

            _subject.Log(SessionSwitchReason.SessionUnlock);

            _mockDataProvider.Verify(x => x.UpdateEvents(It.IsAny<DateTime>(), It.IsAny<WorkEvent>()));
        }

        [TestMethod]
        public void TestLog_LogsLogonEvent()
        {

            _subject.Log(SessionSwitchReason.SessionLogon);

            _mockDataProvider.Verify(x => x.UpdateEvents(It.IsAny<DateTime>(), It.IsAny<WorkEvent>()));
        }

        [TestMethod]
        public void TestLog_LogsLogoffEvent()
        {

            _subject.Log(SessionSwitchReason.SessionLogoff);

            _mockDataProvider.Verify(x => x.UpdateEvents(It.IsAny<DateTime>(), It.IsAny<WorkEvent>()));
        }

        [TestMethod]
        public void TestLog_IgnoresEvents()
        {

            _subject.Log(SessionSwitchReason.RemoteConnect);
            _subject.Log(SessionSwitchReason.ConsoleConnect);
            _subject.Log(SessionSwitchReason.SessionRemoteControl);

            _mockDataProvider.Verify(x => x.UpdateEvents(It.IsAny<DateTime>(), It.IsAny<WorkEvent>()), Times.Never);
        }
    }
}
