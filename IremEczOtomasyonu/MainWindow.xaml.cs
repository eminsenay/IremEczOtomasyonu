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
using IremEczOtomasyonu.BL;
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
        private readonly Model1Container _dbContext;

        public MainWindow()
        {
            InitializeComponent();
            _dbContext = new Model1Container();
            _customersUserControl = new UserControlCustomers(_dbContext);
            customersTabItem.Content = _customersUserControl;
            _productsUserControl = new UserControlProducts(_dbContext);
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

        private void ProductSaleButton_Click(object sender, RoutedEventArgs e)
        {
            _customersUserControl.ExecuteCustomerSale(null);
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ProductTotalPriceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Many different queries are not the optimal way of doing it, but it works. 
            // Consider changing it if its performance is not satisifactory
            int numDifferentBrands = (from p in _dbContext.Products select p.Brand).Distinct().Count();
            int productCount = _dbContext.Products.Sum(x => x.NumItems);
            decimal totalSalePrice = _dbContext.Products.Sum(x => (x.NumItems*x.CurrentSellingPrice)) ?? 0;
            string messageToShow = string.Format(
                "Stoktaki {0} farklı markanın toplam {1} ürününün güncel satış fiyatı toplamı {2:0.00} TL'dir.",
                numDifferentBrands, productCount, totalSalePrice);
            MessageBox.Show(messageToShow, "Stok değeri");
        }

        private void ProductSaleDisplayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaleListWindow saleListWindow = new SaleListWindow(_dbContext)
                                            {
                                                Owner = this,
                                                WindowStartupLocation = WindowStartupLocation.CenterOwner
                                            };
            saleListWindow.ShowDialog();
        }
    }
}
