using System;
using System.Windows;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;
using CrashReporterDotNET;
using CefSharp;
using CefSharp.Wpf;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region CrashReporter

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.Current.DispatcherUnhandledException += DispatcherOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            SendReport(unobservedTaskExceptionEventArgs.Exception);
            Quit();
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            SendReport(dispatcherUnhandledExceptionEventArgs.Exception);
            Quit();
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            SendReport((Exception)unhandledExceptionEventArgs.ExceptionObject);
            Quit();
        }

        private static void SendReport(Exception exception, string developerMessage = "", bool silent = false)
        {
            var reportCrash = new ReportCrash("sampsa.kaskela@gmail.com")
            {
                DeveloperMessage = developerMessage
            };
            reportCrash.Silent = silent;
            reportCrash.Send(exception);
        }

        private static void Quit()
        {
            Environment.Exit(0);
        }

        #endregion
    }
}
