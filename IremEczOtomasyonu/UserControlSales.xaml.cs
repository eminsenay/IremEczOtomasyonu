using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private Window _parentWindow;

        private Window ParentWindow
        {
            get { return _parentWindow ?? (_parentWindow = Window.GetWindow(this)); }
        }

        public ProductSale CurrentProductSale { get; set; }

        public event ChangedEventHandler CurrentProductSaleChanged;

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
        /// Event handler for product add button. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductAddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewProduct();
        }

        /// <summary>
        /// Adds a new product to the purchase. Checks if product is given with barcode or the name and brand.
        /// If the user entered a barcode and it cannot be found in the database, an error is raised.
        /// An error is also raised if the user entered a product name and brand, 
        /// and they don't correspond to a product.
        /// </summary>
        private void AddNewProduct()
        {
            if ((barcodeTextBox == null || string.IsNullOrEmpty(barcodeTextBox.Text) &&
                 (productBrandAndNameAutoCompleteBox == null ||
                  string.IsNullOrEmpty(productBrandAndNameAutoCompleteBox.Text))))
            {
                return;
            }

            Product selectedProduct;
            bool barcodeGiven = false;
            if (!string.IsNullOrEmpty(barcodeTextBox.Text))
            {
                barcodeGiven = true;
                // Add new item using barcode
                selectedProduct = ObjectCtx.Context.Products.FirstOrDefault(p => p.Barcode == barcodeTextBox.Text);
                if (selectedProduct == null)
                {
                    MessageBox.Show("Girmiş olduğunuz barkod sistemde kayıtlı değil.", "Ürün uyarısı",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    barcodeTextBox.SelectAll();
                    return;
                }
            }
            else
            {
                // Add new item using product


                // Check if the product exists
                if (productBrandAndNameAutoCompleteBox.SelectedItem == null)
                {
                    return;
                }
                selectedProduct = productBrandAndNameAutoCompleteBox.SelectedItem as Product;
                if (selectedProduct == null)
                {
                    return;
                }
            }

            ExpirationDate selectedExpDate = selectedProduct.ExpirationDates.FirstOrDefault();
            if (selectedExpDate == null)
            {
                Debug.Fail("No Expiration date can be found for the product.");
                return;
            }
            SaleItem newItem = new SaleItem
                                   {
                                       Id = Guid.NewGuid(),
                                       Product = selectedProduct,
                                       NumSold = 1,
                                       ProductSale = CurrentProductSale,
                                       ExDate = selectedExpDate.ExDate,
                                       UnitPrice = selectedProduct.CurrentSellingPrice ?? 0
                                   };
            CurrentProductSale.SaleItems.Add(newItem);
            newItem.Product.NumItems -= newItem.NumSold;
            selectedExpDate.NumItems -= newItem.NumSold;

            // Make prevnumsold equal. Values should only be different when the user manually changes the numsold.
            newItem.PrevNumSold = newItem.NumSold;

            UpdateTotalPrice();

            if (barcodeGiven)
            {
                barcodeTextBox.Text = string.Empty;
                barcodeTextBox.Focus();
            }
            else
            {
                productBrandAndNameAutoCompleteBox.Text = string.Empty;
                productBrandAndNameAutoCompleteBox.Focus();
            }

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
                // User has changed the number of items cell.

                int diff = currItem.NumSold - currItem.PrevNumSold;

                currItem.Product.NumItems -= diff;
                ExpirationDate selectedExpDate = currItem.Product.ExpirationDates.FirstOrDefault(
                    x => x.ExDate == currItem.ExDate);
                if (selectedExpDate != null)
                {
                    selectedExpDate.NumItems -= diff;
                    if (selectedExpDate.NumItems < 0)
                    {
                        MessageBox.Show("Ürünün geçerli son kullanma tarihine sahip stok miktarı sıfırın altına indi. " +
                            "Lütfen stok bilgilerini tekrar kontrol edin.", "Stok uyarısı",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    // User has modified the product's expiration date after selling it
                    // We don't track expiration date modifications, so create a new expirationDate object with 
                    // the original date and notify the user.

                    MessageBox.Show("Ürünü sisteme girdikten sonra son kullanma tarihlerini değiştirmişsiniz.\n" +
                        "Eklenen/çıkarılan ürünler yine de orijinal son kullanma tarihini kullanacaklardır.\n" +
                        "Bu penceredeki işinizin bitiminden sonra ürün detaylarına girip son kullanma tarihlerini " +
                        "kontrol etmeniz faydalı olabilir.", "Ürün son kullanma tarihi uyarısı", MessageBoxButton.OK);
                    ExpirationDate orgExpirationDate = new ExpirationDate
                    {
                        Id = Guid.NewGuid(),
                        ExDate = currItem.ExDate,
                        NumItems = -diff,
                        Product = currItem.Product
                    };
                    currItem.Product.ExpirationDates.Add(orgExpirationDate);
                }
                currItem.PrevNumSold = currItem.NumSold;
            }

            // Set the flag to false for future edits
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
            if (e.OriginalSource is DataGridCell && e.Key == Key.Delete)
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
            if (selectedExpDate != null)
            {
                selectedExpDate.NumItems += saleItem.NumSold;
            }
            else
            {
                // User has modified the product's expiration date after selling it
                // We don't track expiration date modifications, so create a new expirationDate object with 
                // the original date and notify the user.

                MessageBox.Show("Ürünü sisteme girdikten sonra son kullanma tarihlerini değiştirmişsiniz.\n" +
                    "Eklenen/çıkarılan ürünler yine de orijinal son kullanma tarihini kullanacaklardır.\n" +
                    "Bu penceredeki işinizin bitiminden sonra ürün detaylarına girip son kullanma tarihlerini " +
                    "kontrol etmeniz faydalı olabilir.", "Ürün son kullanma tarihi uyarısı", MessageBoxButton.OK);
                ExpirationDate orgExpirationDate = new ExpirationDate
                {
                    Id = Guid.NewGuid(),
                    ExDate = saleItem.ExDate,
                    NumItems = saleItem.NumSold,
                    Product = product
                };
                product.ExpirationDates.Add(orgExpirationDate);
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
                Owner = ParentWindow
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
        /// Checks the number of bought items in the stock and produces an error message if stock of the items are
        /// below 0.
        /// </summary>
        /// <returns></returns>
        public string StockControl()
        {
            if (CurrentProductSale == null)
            {
                return null;
            }
            StringBuilder errorMsg = null;
            foreach (SaleItem saleItem in CurrentProductSale.SaleItems)
            {
                if (saleItem.Product.NumItems - saleItem.NumSold >= 0)
                {
                    continue;
                }

                if (errorMsg == null)
                {
                    errorMsg = new StringBuilder(
                        "Şu ürünlerin stok miktarı 0'ın altında: " + saleItem.Product.Name);
                }
                else
                {
                    errorMsg.Append(", " + saleItem.Product.Name);
                }
            }
            if (errorMsg == null)
            {
                return null;
            }
            errorMsg.AppendLine();
            errorMsg.AppendLine("Lütfen bunların sistemdeki stoğunu tekrar kontrol edin.");
            return errorMsg.ToString();
        }
        

        /// <summary>
        /// Removes all added sale items from the product sale.
        /// </summary>
        public void RevertProductSale()
        {
            for (int i = 0; i < productSaleDataGrid.Items.Count; i++)
            {
                SaleItem saleItem = productSaleDataGrid.Items[i] as SaleItem;
                DeleteSaleItem(saleItem);
                i--;
            }
        }

        private void CustomerNameTextBoxDeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentProductSale.Customer = null;
            OnCurrentProductSaleChanged();
        }

        private void ExpirationDateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0 || e.RemovedItems.Count <= 0)
            {
                return;
            }
            DateTime prevSelection = (DateTime)e.RemovedItems[0];
            DateTime currSelection = (DateTime)e.AddedItems[0];

            SaleItem currItem = productSaleDataGrid.SelectedItem as SaleItem;
            if (currItem == null)
            {
                return;
            }

            // User changed the expiration date of the sale item. 
            // Change associated expiration date objects accordingly.

            ExpirationDate oldExpDate =
                currItem.Product.ExpirationDates.FirstOrDefault(x => x.ExDate == prevSelection);
            ExpirationDate newExpDate =
                currItem.Product.ExpirationDates.FirstOrDefault(x => x.ExDate == currSelection);

            if (oldExpDate == null || newExpDate == null)
            {
                // They cannot be null since both of them are selected from the combo box.
                Debug.Fail("Expiration date of the sale item cannot be found");
                return;
            }
            oldExpDate.NumItems += currItem.NumSold;
            newExpDate.NumItems -= currItem.NumSold;
            if (newExpDate.NumItems < 0)
            {
                MessageBox.Show("Ürünün geçerli son kullanma tarihine sahip stok miktarı sıfırın altına indi. " +
                                "Lütfen stok bilgilerini tekrar kontrol edin.", "Stok uyarısı",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            productBrandAndNameAutoCompleteBox.DataContext = ObjectCtx.Context.Products.OrderBy(p => p.Brand);
            productBrandAndNameAutoCompleteBox.ItemFilter += ItemFilter;
        }

        private bool ItemFilter(string search, object item)
        {
            Product p = item as Product;
            if (p == null)
            {
                return false;
            }
            string[] searchTokens = search.Split(' ', '-');
            foreach (string searchToken in searchTokens)
            {
                if (!p.Name.ToLowerInvariant().Contains(searchToken.ToLowerInvariant()) && 
                    !p.Brand.ToLowerInvariant().Contains(searchToken.ToLowerInvariant()))
                {
                    return false;
                }
            }
            return true;
        }

        private void ProductBrandAndNameAutoCompleteBox_EnterKeyDown(object obj)
        {
            AddNewProduct();
        }
    }
}
