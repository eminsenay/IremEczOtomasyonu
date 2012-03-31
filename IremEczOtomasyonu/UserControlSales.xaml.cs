using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Globalization;
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
using IremEczOtomasyonu.BL;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for UserControlSales.xaml
    /// </summary>
    public partial class UserControlSales : UserControl
    {
        private bool _isManualEditCommit;
        private ProductSale _currentProductSale;
        private Model1Container _dbContext;

        public Model1Container DbContext
        {
            get { return _dbContext ?? (_dbContext = new Model1Container()); }
            set { _dbContext = value; }
        }
        public ProductSale CurrentProductSale
        {
            get { return _currentProductSale; }
            set 
            { 
                _currentProductSale = value;
                saleGrid.DataContext = CurrentProductSale;
                productSaleDataGrid.ItemsSource = CurrentProductSale.SaleItems;
                UpdateTotalPriceTextBox();
            }
        }

        public UserControlSales()
        {
            InitializeComponent();
        }

        public UserControlSales(Model1Container dbContext, ProductSale productSale)
        {
            InitializeComponent();

            DbContext = dbContext;
            CurrentProductSale = productSale;
        }

        /// <summary>
        /// Adds a new product to the purchase. If the barcode cannot be found in the database, an error is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (barcodeTextBox == null || string.IsNullOrEmpty(barcodeTextBox.Text))
            {
                return;
            }
            Product newProduct = DbContext.Products.FirstOrDefault(p => p.Barcode == barcodeTextBox.Text);
            if (newProduct == null)
            {
                MessageBox.Show("Girmiş olduğunuz barkod sistemde kayıtlı değil.", "Ürün uyarısı", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                barcodeTextBox.SelectAll();
                return;
            }
            CurrentProductSale.SaleItems.Add(new SaleItem
            {
                Product = newProduct,
                NumSold = 1,
                ProductSale = CurrentProductSale,
                ExpDate = newProduct.ExpirationDates.FirstOrDefault(),
                UnitPrice = newProduct.CurrentSellingPrice ?? 0
            });
            UpdateTotalPriceTextBox();

            barcodeTextBox.Text = string.Empty;
            barcodeTextBox.Focus();
        }

        /// <summary>
        /// CellEditEnding event handler of the datagrid. Commits the change and updates the total price textbox.
        /// Manual change committing is required to update the saleitem price if the user presses for ex. tab button
        /// instead of return.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductSaleDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_isManualEditCommit)
            {
                return;
            }
            _isManualEditCommit = true;
            productSaleDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            _isManualEditCommit = false;
            UpdateTotalPriceTextBox();
        }

        /// <summary>
        /// Manual handle for the Delete key event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductSaleDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteSelectedSaleItem();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event handler for the delete context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatagridDeleteSaleItemMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedSaleItem();
        }

        /// <summary>
        /// Deletes the selected sale item from the datagrid without any questions.
        /// </summary>
        private void DeleteSelectedSaleItem()
        {
            SaleItem saleItem = productSaleDataGrid.SelectedItem as SaleItem;
            if (saleItem == null)
            {
                return;
            }

            CurrentProductSale.SaleItems.Remove(saleItem);
            UpdateTotalPriceTextBox();
        }

        /// <summary>
        /// Updates the total price text block with the sum of the purchases.
        /// </summary>
        private void UpdateTotalPriceTextBox()
        {
            decimal totalPrice = CurrentProductSale.SaleItems.Sum(x => (x.NumSold * x.UnitPrice));
            totalPriceTextBox.Text = totalPrice.ToString(CultureInfo.CurrentCulture);
        }

        private void CustomerFindButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow(DbContext)
            {
                SelectedCustomer = CurrentProductSale.Customer,
                Owner = Parent as Window,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            customerListWindow.ShowDialog();
            if (customerListWindow.DialogResult == true)
            {
                CurrentProductSale.Customer = customerListWindow.SelectedCustomer;
            }
        }

        public string Validate()
        {
            if (Validation.GetHasError(totalPriceTextBox) || UIUtilities.HasDataGridErrors(productSaleDataGrid))
            {
                return "Girdiğiniz bazı bilgiler eksik ya da hatalı. \n Lütfen düzeltip tekrar deneyin.";
            }
            return null;
        }
    }
}
