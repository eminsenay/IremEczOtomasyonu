using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UserControlCustomers _customersUserControl;
        private readonly UserControlProducts _productsUserControl;

        public MainWindow()
        {
            InitializeComponent();
            _customersUserControl = new UserControlCustomers();
            customersTabItem.Content = _customersUserControl;
            _productsUserControl = new UserControlProducts();
            productsTabItem.Content = _productsUserControl;

            _customersUserControl.searchFirstNameInfoTextBox.Focus();
        }

        /// <summary>
        /// On a close request, checks if any unsaved changes exist and warns the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_customersUserControl.AllChangesSaved)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show(
                "Kaydedilmemiş değişiklikleriniz var. Yine de çıkmak istiyor musunuz?", 
                "Uyarı", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        
    }
}
