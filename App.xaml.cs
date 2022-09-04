using System.Configuration;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using WhatIfF1.Logging;

namespace WhatIfF1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string Version { get; }

        public string AppName { get; }

        public App()
        {
            AppName = "WhatIF1";
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Set Syncfusion UI library license key
            string syncfusionKey = ConfigurationManager.AppSettings["syncfusionLicenseKey"];

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionKey);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Instance.Exception(e.Exception);
        }
    }
}
