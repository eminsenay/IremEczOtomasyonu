using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IremEczOtomasyonu.BL;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Data.Objects;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for SaleListWindow.xaml
    /// </summary>
    public partial class SaleListWindow : Window
    {
        private ObservableCollection<ProductSale> _productSaleColl;
        private readonly List<ProductSale> _changedProductSales;
        public List<ProductSale> ChangedProductSalesPersistent { get; private set; }

        public SaleListWindow()
        {
            InitializeComponent();
            _changedProductSales = new List<ProductSale>();
            ChangedProductSalesPersistent = new List<ProductSale>();

            userControlSales.CurrentProductSaleChanged += OnCurrentProductSaleChanged;
        }

        void OnCurrentProductSaleChanged()
        {
            _changedProductSales.Add(userControlSales.CurrentProductSale);
            ChangedProductSalesPersistent.Add(userControlSales.CurrentProductSale);
            applyButton.IsEnabled = true;
            DataGridRow dataGridRow = productSalesDataGrid.ItemContainerGenerator.ContainerFromIndex(
                productSalesDataGrid.SelectedIndex) as DataGridRow;
            if (dataGridRow == null)
            {
                return;
            }
            dataGridRow.Background = Brushes.Aquamarine;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load data into ProductSales. You can modify this code as needed.
            CollectionViewSource productSalesViewSource = ((CollectionViewSource)(
                FindResource("productSalesViewSource")));
            _productSaleColl = new ObservableCollection<ProductSale>(ObjectCtx.Context.ProductSales.OrderByDescending(
                x => x.SaleDate));
            productSalesViewSource.Source = _productSaleColl;

            userControlSales.saleGrid.DataContext = productSalesViewSource;
        }

        private void ProductSalesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // The following binding doesn't work properly. 
            // CurrentProductSale keeps being null, so update the CurrentProductSale by hand

            //Binding binding = new Binding
            //                  {
            //                      Source = userControlSales.CurrentProductSale,
            //                      Mode = BindingMode.OneWayToSource
            //                  };
            //productSalesDataGrid.SetBinding(Selector.SelectedItemProperty, binding);

            userControlSales.CurrentProductSale = productSalesDataGrid.SelectedItem as ProductSale;

        }

        private void ProductSalesDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteSelectedProductSale();
                e.Handled = true;
            }
        }

        private void DatagridDeleteProductSaleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedProductSale();
        }

        private void DeleteSelectedProductSale()
        {
            ProductSale selectedSale = productSalesDataGrid.SelectedItem as ProductSale;
            if (selectedSale == null)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show("Seçili satışı silmek istediğinizden emin misiniz?",
                "Satış silme onayı", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            // Add the Sale Items to the stock again.
            foreach (SaleItem saleItem in new List<SaleItem>(selectedSale.SaleItems))
            {
                saleItem.Product.NumItems += saleItem.NumSold;
                ExpirationDate exDateObj = saleItem.Product.ExpirationDates.FirstOrDefault(
                    x => x.ExDate == saleItem.ExDate);
                if (exDateObj != null)
                {
                    exDateObj.NumItems++;
                }
                else
                {
                    Debug.Fail("Expiration Date of the sale item cannot be found.");
                }

                ObjectCtx.Context.SaleItems.DeleteObject(saleItem);
            }

            // Delete the product sale
            _productSaleColl.Remove(selectedSale);
            ObjectCtx.Context.DeleteObject(selectedSale);

            applyButton.IsEnabled = true;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // validation check
            string validationMsg = userControlSales.Validate();
            if (validationMsg != null)
            {
                MessageBox.Show(validationMsg, "Satış uyarısı", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string stockControlMsg = userControlSales.StockControl();
            if (stockControlMsg != null)
            {
                MessageBox.Show(stockControlMsg, "Satış uyarısı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            ObjectCtx.Context.SaveChanges();

            foreach (ProductSale productSale in _changedProductSales)
            {
                DataGridRow dataGridRow = productSalesDataGrid.ItemContainerGenerator.ContainerFromItem(
                    productSale) as DataGridRow;
                if (dataGridRow == null)
                {
                    continue;
                }
                dataGridRow.Background = Brushes.White;
            }

            _changedProductSales.Clear();
            
            applyButton.IsEnabled = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // validation check
            string validationMsg = userControlSales.Validate();
            if (validationMsg != null)
            {
                MessageBox.Show(validationMsg, "Satış uyarısı", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string stockControlMsg = userControlSales.StockControl();
            if (stockControlMsg != null)
            {
                MessageBox.Show(stockControlMsg, "Satış uyarısı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            ObjectCtx.Context.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}
