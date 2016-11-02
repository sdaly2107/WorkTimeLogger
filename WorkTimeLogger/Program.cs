using NLog;
using System;
using Topshelf;

namespace WorkTimeLogger
{
    public static class Program
    {
        static ILogger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_CurrentDomain_UnhandledException);

            HostFactory.Run(x =>
            {
                x.Service<Service>(s =>
                {
                    s.ConstructUsing(name => new Service());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenSessionChanged((tc, e) => tc.SessionChange(e));
                });

                x.RunAsLocalSystem();
                x.EnableSessionChanged();
                
                x.SetDescription("Logs work time based on session lock/unlock events");
                x.SetDisplayName("Work Time Logger");
                x.SetServiceName("WorkTimeLogger");
            });
        }

      

        private static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Warn("Unhandled exception");
            _logger.Error(e.ExceptionObject);
        }
    }
}
