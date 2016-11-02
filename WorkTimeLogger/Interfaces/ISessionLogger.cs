using Microsoft.Win32;

namespace WorkTimeLogger.Interfaces
{
    public interface ISessionLogger
    {
        void Log(SessionSwitchReason reason);
    }
}
