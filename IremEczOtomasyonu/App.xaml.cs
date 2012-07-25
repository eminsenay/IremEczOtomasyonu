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
            MessageBox.Show(e.Exception.Message, "Uncaught Exception",
                MessageBoxButton.OK, MessageBoxImage.Error);
            MessageBox.Show(e.Exception.ToString(), "Uncaught Exception",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
