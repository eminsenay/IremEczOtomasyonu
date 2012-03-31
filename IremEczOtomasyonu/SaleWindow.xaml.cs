﻿using System;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IremEczOtomasyonu.BL;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        private readonly Model1Container _dbContext;
        
        private bool _isManualEditCommit;

        public ProductSale CurrentProductSale { get; private set; }

        /// <summary>
        /// Initializes a new sale window
        /// </summary>
        /// <param name="dbContext">Database context to use.</param>
        public SaleWindow(Model1Container dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;

            CurrentProductSale = new ProductSale
                                 {
                                     SaleItems = new EntityCollection<SaleItem>(),
                                     SaleDate = DateTime.Now, 
                                     TotalPrice = 0
                                 };

            saleGrid.DataContext = CurrentProductSale;
            productSaleDataGrid.ItemsSource = CurrentProductSale.SaleItems;

            UpdateTotalPriceTextBox();
        }

        /// <summary>
        /// Initializes a new sale window for the given customer.
        /// </summary>
        /// <param name="dbContext">Database context to use.</param>
        /// <param name="customer">Customer who is buying the products</param>
        public SaleWindow(Model1Container dbContext, Customer customer): this(dbContext)
        {
            CurrentProductSale.Customer = customer;
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
            //SaleItems.Add(new SaleItem {Product = newProduct, NumSold = 1, ProductSale = CurrentProductSale});
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
            //CurrentProductSale.TotalPrice = totalPrice;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // validation check
            if (Validation.GetHasError(totalPriceTextBox) || UIUtilities.HasDataGridErrors(productSaleDataGrid))
            {
                MessageBox.Show("Girdiğiniz bazı bilgiler eksik ya da hatalı. \n Lütfen düzeltip tekrar deneyin.",
                                "Satış uyarısı", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CurrentProductSale.SaleItems.Count == 0)
            {
                DialogResult = true;
                Close();
                return;
            }

            CurrentProductSale.Id = Guid.NewGuid();

            // Set entity expiration dates and ids manually
            foreach (SaleItem saleItem in CurrentProductSale.SaleItems)
            {
                saleItem.ExDate = saleItem.ExpDate.ExDate;
                // Decrease the item count from products & expiration date tables
                saleItem.Product.NumItems -= saleItem.NumSold;
                saleItem.ExpDate.NumItems -= saleItem.NumSold;
                saleItem.Id = Guid.NewGuid();
            }
            
            _dbContext.AddToProductSales(CurrentProductSale);
            _dbContext.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void CustomerFindButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow(_dbContext)
                                                    {
                                                        SelectedCustomer = CurrentProductSale.Customer,
                                                        Owner = this,
                                                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                                                    };
            customerListWindow.ShowDialog();
            if (customerListWindow.DialogResult == true)
            {
                CurrentProductSale.Customer = customerListWindow.SelectedCustomer;
            }
        }
    }
}
