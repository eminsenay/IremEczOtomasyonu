using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Uncaught Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            MessageBox.Show(e.Exception.ToString(), "Uncaught Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Sets the DataDirectory to use the correct sdf file for debug and release (setup) builds. 
        /// Debug build should directly use the one inside the solution whereas release build should use the one 
        /// copied under the %appdata% directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Connection string must look like the following to use the DataDirectory (don't forget the pipes):
        /// &lt;connectionStrings&gt;
        ///  &lt;add name="DataContext" connectionString="Data source=|DataDirectory|Database.sdf;"
        ///       providerName="System.Data.SqlServerCe.4.0"/&gt;
        /// &lt;/connectionStrings&lt;
        /// </remarks>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string dbDir;
#if !DEBUG
            // The following code is necessary to use the sdf file located at the %appdata% directory for setup builds
            dbDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Connection string must look like the following (don't forget the pipes):

            
#else
            System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
            string baseDir = System.IO.Path.GetDirectoryName(a.Location);
// ReSharper disable AssignNullToNotNullAttribute
            dbDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDir, "..\\.."));
// ReSharper restore AssignNullToNotNullAttribute
#endif
            AppDomain.CurrentDomain.SetData("DataDirectory", dbDir);
        }
    }
}
