using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        private readonly Model1Container _dbContext;
        private readonly Customer _customer;
        
        private bool _isManualEditCommit;

        public ObservableCollection<SaleItem> SaleItems { get; set; }

        /// <summary>
        /// Initializes a new sale window
        /// </summary>
        /// <param name="dbContext">Database context to use.</param>
        public SaleWindow(Model1Container dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            SaleItems = new ObservableCollection<SaleItem>();

            productSaleDataGrid.ItemsSource = SaleItems;
            UpdateTotalPriceTextBlock();
        }

        /// <summary>
        /// Initializes a new sale window for the given customer.
        /// </summary>
        /// <param name="dbContext">Database context to use.</param>
        /// <param name="customer">Customer who is buying the products</param>
        public SaleWindow(Model1Container dbContext, Customer customer): this(dbContext)
        {
            _customer = customer;
            customerNameTextBlock.Text = _customer.FirstName + " " + _customer.LastName;
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
            Product newProduct = _dbContext.Products.FirstOrDefault(p => p.Barcode == barcodeTextBox.Text);
            if (newProduct == null)
            {
                MessageBox.Show("Girmiş olduğunuz barkod sistemde kayıtlı değil.", "Ürün uyarısı", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                barcodeTextBox.SelectAll();
                return;
            }
            SaleItems.Add(new SaleItem { Product = newProduct, NumSold = 1 });
            totalPriceTextBlock.Text = SaleItems.Sum(x => x.Price).ToString(CultureInfo.InvariantCulture);

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
            UpdateTotalPriceTextBlock();
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

            SaleItems.Remove(saleItem);
            UpdateTotalPriceTextBlock();
        }

        /// <summary>
        /// Updates the total price text block with the sum of the purchases.
        /// </summary>
        private void UpdateTotalPriceTextBlock()
        {
            totalPriceTextBlock.Text = SaleItems.Sum(x => x.Price).ToString(CultureInfo.InvariantCulture);
        }
    }
}
