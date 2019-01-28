using System;
using System.Data.SqlServerCe;
using System.Linq;
using System.Windows;
using IremEczOtomasyonu.BL;

namespace IremEczOtomasyonu.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly UserControlCustomers _customersUserControl;
        private readonly UserControlProducts _productsUserControl;

        public MainWindow()
        {
            InitializeComponent();
            _customersUserControl = new UserControlCustomers();
            _productsUserControl = new UserControlProducts();
            dockPanel.Children.Add(_customersUserControl);
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
            _customersUserControl.ExecuteCustomerSale(_customersUserControl.customersDataGrid.SelectedItem as Customer);
        }

        private void ProductSaleWithoutCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            _customersUserControl.ExecuteCustomerSale(null);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ProductTotalPriceButton_Click(object sender, RoutedEventArgs e)
        {
            int numDifferentBrands, productCount;
            decimal totalSalePrice;
            // Many different queries are not the optimal way of doing it, but it works. 
            // Consider changing it if its performance is not satisfactory
            if (!ObjectCtx.Context.Products.Any())
            {
                // No products exist now
                numDifferentBrands = 0;
                productCount = 0;
                totalSalePrice = 0;
            }
            else 
            {
                numDifferentBrands = (from p in ObjectCtx.Context.Products select p.Brand).Distinct().Count();
                productCount = ObjectCtx.Context.Products.Sum(x => x.NumItems);
                totalSalePrice = ObjectCtx.Context.Products.Sum(x => (x.NumItems * x.CurrentSellingPrice)) ?? 0;
            }
            
            string messageToShow = string.Format(
                "Stoktaki {0} farklı markanın toplam {1} ürününün güncel satış fiyatı toplamı {2:0.00} TL'dir.",
                numDifferentBrands, productCount, totalSalePrice);
            MessageBox.Show(messageToShow, "Stok değeri");
        }

        private void PreviousSalesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_customersUserControl.AllChangesSaved)
            {
                MessageBox.Show("Bu işlemden önce yaptığınız değişiklikleri kaydetmeniz gerekmektedir.",
                                "Değişiklik uyarısı", MessageBoxButton.OK);
                return;
            }
            SaleListWindow saleListWindow = new SaleListWindow { Owner = this };
            if (saleListWindow.ShowDialog() != true)
            {
                ObjectCtx.Reload();
                _customersUserControl.Reload();
                _productsUserControl.Reload();
            }
            //else
            //{
            //    _customersUserControl._saleItemsView.Refresh();
            //}
        }

        private void NumberOfPurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            ProductSaleCountWindow productSaleCountWindow = new ProductSaleCountWindow { Owner = this };
            productSaleCountWindow.ShowDialog();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutBox aboutBox = new AboutBox(this);
            aboutBox.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if database exists
            if (!CheckDatabaseConnection())
            {
                Environment.Exit(-1);
            }
            else
            {
                // Check the expiration dates
                DateTime oneMonthLater = DateTime.Today + new TimeSpan(30, 0, 0, 0);
                if (ObjectCtx.Context.ExpirationDates.Any(x => x.NumItems > 0 && oneMonthLater >= x.ExDate))
                {
                    MessageBox.Show(this,
                        "Son kullanma tarihi 30 gün içinde dolacak olan ürünleriniz var. Detaylı bilgi için: Ekstra --> Son Kullanma Tarihleri Yaklaşan Ürünler");
                }
            }
        }

        private void ProductsToBeExpiredButton_Click(object sender, RoutedEventArgs e)
        {
            IncomingExpirationsWindow incomingExpirationsWindow = new IncomingExpirationsWindow { Owner = this };
            incomingExpirationsWindow.ShowDialog();
        }

        private void PreviousPurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_customersUserControl.AllChangesSaved)
            {
                MessageBox.Show("Bu işlemden önce yaptığınız değişiklikleri kaydetmeniz gerekmektedir.",
                                "Değişiklik uyarısı", MessageBoxButton.OK);
                return;
            }
            PurchaseListWindow purchaseListWindow = new PurchaseListWindow { Owner = this };
            if (purchaseListWindow.ShowDialog() != true)
            {
                ObjectCtx.Reload();
                _customersUserControl.Reload();
                _productsUserControl.Reload();
            }
        }

        /// <summary>
        /// Checks if the database is at its given place at the datasource.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Don't forget to modify the connection string if you modify the app.config. 
        /// Getting it dynamically as 
        /// "string connectionString = "DataSource=" + ObjectCtx.Context.Connection.DataSource;" 
        /// does only work when the sql server ce is installed, so it is avoided here.</remarks>
        private bool CheckDatabaseConnection()
        {
            // Check if database exists
            if (ObjectCtx.Context.DatabaseExists())
            {
                return true;
            }
            const string ConnectionString = "DataSource=|DataDirectory|IremEczDermokozmetikDb.sdf";
            MessageBox.Show(this, "Veritabanına ulaşılamıyor. Olması beklenen yer: " +
                            PathFromConnectionString(ConnectionString));
            return false;
        }

        private static string PathFromConnectionString(string connectionString)
        {
            SqlCeConnectionStringBuilder sb = new SqlCeConnectionStringBuilder(GetFullConnectionString(connectionString));
            return sb.DataSource;
        }

        private static string GetFullConnectionString(string connectionString)
        {
            using (SqlCeReplication repl = new SqlCeReplication())
            {
                repl.SubscriberConnectionString = connectionString;
                return repl.SubscriberConnectionString;
            }
        }

        private void CustomersButton_Click(object sender, RoutedEventArgs e)
        {
            dockPanel.Children.Clear();
            dockPanel.Children.Add(_customersUserControl);
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            dockPanel.Children.Clear();
            dockPanel.Children.Add(_productsUserControl);
        }
    }
}
