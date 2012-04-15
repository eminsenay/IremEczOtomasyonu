using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Diagnostics;
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
    // A delegate type for hooking up change notifications.
    public delegate void ChangedEventHandler();

    /// <summary>
    /// Interaction logic for UserControlSales.xaml
    /// </summary>
    public partial class UserControlSales : UserControl
    {
        private bool _isManualEditCommit;
        public event ChangedEventHandler CurrentProductSaleChanged;

        public ProductSale CurrentProductSale { get; set; }

        public UserControlSales()
        {
            InitializeComponent();
        }

        public void OnCurrentProductSaleChanged()
        {
            if (CurrentProductSaleChanged != null)
            {
                CurrentProductSaleChanged();
            }
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
            Product newProduct = ObjectCtx.Context.Products.FirstOrDefault(p => p.Barcode == barcodeTextBox.Text);
            if (newProduct == null)
            {
                MessageBox.Show("Girmiş olduğunuz barkod sistemde kayıtlı değil.", "Ürün uyarısı", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                barcodeTextBox.SelectAll();
                return;
            }

            SaleItem newItem = new SaleItem
                               {
                                   Id = Guid.NewGuid(),
                                   Product = newProduct,
                                   NumSold = 1,
                                   ProductSale = CurrentProductSale,
                                   ExDate = newProduct.ExpirationDates.Select(x => x.ExDate).FirstOrDefault(),
                                   UnitPrice = newProduct.CurrentSellingPrice ?? 0
                               };
            CurrentProductSale.SaleItems.Add(newItem);
            newItem.Product.NumItems -= newItem.NumSold;

            ExpirationDate selectedExpDate = newItem.Product.ExpirationDates.FirstOrDefault(
                    x => x.ExDate == newItem.ExDate);
            Debug.Assert(selectedExpDate != null, "Expiration Date of the sale item cannot be found.");
            if (selectedExpDate != null)
            {
                selectedExpDate.NumItems -= newItem.NumSold;
            }

            // Make prevnumsold equal. Values should only be different when the user manually changes the numsold.
            newItem.PrevNumSold = newItem.NumSold;
            
            UpdateTotalPrice();

            barcodeTextBox.Text = string.Empty;
            barcodeTextBox.Focus();

            OnCurrentProductSaleChanged();
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
            SaleItem currItem = e.Row.Item as SaleItem;
            Debug.Assert(currItem != null, "Associated sale item cannot be retrieved.");
            if (currItem != null && currItem.PrevNumSold != currItem.NumSold)
            {
                int diff = currItem.NumSold - currItem.PrevNumSold;

                currItem.Product.NumItems -= diff;
                ExpirationDate selectedExpDate = currItem.Product.ExpirationDates.FirstOrDefault(
                    x => x.ExDate == currItem.ExDate);
                Debug.Assert(selectedExpDate != null, "Expiration Date of the sale item cannot be found.");
                if (selectedExpDate != null)
                {
                    selectedExpDate.NumItems -= diff;
                }
                currItem.PrevNumSold = currItem.NumSold;
            }

            _isManualEditCommit = false;
            UpdateTotalPrice();

            OnCurrentProductSaleChanged();
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
            DeleteSaleItem(saleItem);
        }

        /// <summary>
        /// Deletes the given sale item from the datagrid.
        /// </summary>
        private void DeleteSaleItem(SaleItem saleItem)
        {
            if (saleItem == null)
            {
                return;
            }

            CurrentProductSale.SaleItems.Remove(saleItem);

            Product product = saleItem.Product;
            product.NumItems += saleItem.NumSold;
            ExpirationDate selectedExpDate = product.ExpirationDates.FirstOrDefault(
                    x => x.ExDate == saleItem.ExDate);
            Debug.Assert(selectedExpDate != null, "Expiration Date of the sale item cannot be found.");
            if (selectedExpDate != null)
            {
                selectedExpDate.NumItems += saleItem.NumSold;
            }
            product.SaleItems.Remove(saleItem);
            ObjectCtx.Context.Detach(saleItem);

            UpdateTotalPrice();
            OnCurrentProductSaleChanged();
        }

        /// <summary>
        /// Updates the total price text block with the sum of the purchases.
        /// </summary>
        private void UpdateTotalPrice()
        {
            if (CurrentProductSale == null)
            {
                return;
            }
            CurrentProductSale.TotalPrice = CurrentProductSale.SaleItems.Sum(x => (x.NumSold * x.UnitPrice));
        }

        private void CustomerFindButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow
            {
                SelectedCustomer = CurrentProductSale.Customer,
                Owner = Parent as Window,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            customerListWindow.ShowDialog();
            if (customerListWindow.DialogResult == true)
            {
                CurrentProductSale.Customer = customerListWindow.SelectedCustomer;
                OnCurrentProductSaleChanged();
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

        /// <summary>
        /// Removes all added sale items from the product sale.
        /// </summary>
        public void RevertProductSale()
        {
            foreach (SaleItem saleItem in productSaleDataGrid.Items)
            {
                DeleteSaleItem(saleItem);
            }
        }
    }
}
