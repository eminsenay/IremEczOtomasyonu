using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using IremEczOtomasyonu.Models;
using IremEczOtomasyonu.BL;
using Microsoft.EntityFrameworkCore;

namespace IremEczOtomasyonu.UI
{
    /// <summary>
    /// Interaction logic for UserControlProducts.xaml
    /// </summary>
    public partial class UserControlProducts : UserControl
    {
        private ICollectionView _currentView;
        public ObservableCollection<Product> Products { get; private set; }
        private Window _parentWindow;

        private Window ParentWindow
        {
            get { return _parentWindow ?? (_parentWindow = Window.GetWindow(this)); }
        }

        public UserControlProducts()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource productsViewSource = ((CollectionViewSource)(FindResource("ProductsViewSource")));
            Products = new ObservableCollection<Product>(ObjectCtx.Context.Products.Include(
                "ProductPurchases").Include("SaleItems").Include("ExpirationDates"));

            productsViewSource.Source = Products;
            productsViewSource.SortDescriptions.Add(new SortDescription("Brand", ListSortDirection.Ascending));
            productsViewSource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            _currentView = productsViewSource.View;
        }

        public void Reload()
        {
            if (Products == null)
            {
                return;
            }
            Products.Clear();
            foreach (Product product in ObjectCtx.Context.Products.Include("ProductPurchases").Include("SaleItems"))
            {
                Products.Add(product);
            }
        }

        private void AddNewProduct_Click(object sender, RoutedEventArgs e)
        {
            AddNewProductWindow addProductWindow = new AddNewProductWindow { Owner = ParentWindow };
            if (addProductWindow.ShowDialog() == true)
            {
                // A product is added. Refresh the datagrid
                Products.Add(addProductWindow.CurrentProduct);
            }
        }

        private void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            string barcode;
            Product selectedProduct = productsDataGrid.SelectedItem as Product;
            if (selectedProduct != null)
            {
                barcode = selectedProduct.Barcode;
            }
            else
            {
                BarcodeWindow barcodeWindow = new BarcodeWindow { Owner = ParentWindow };
                if (barcodeWindow.ShowDialog() != true)
                {
                    return;
                }
                barcode = barcodeWindow.Barcode;
            }
            
            OpenPurchaseDialog(barcode);
        }

        /// <summary>
        /// Opens a purchase dialog with the given barcode and refreshes the view if a new product is added.
        /// </summary>
        /// <param name="barcode"></param>
        private void OpenPurchaseDialog(string barcode)
        {
            Product product = Products.First(p => p.Barcode == barcode);
            AddPurchaseWindow addPurchaseWindow = new AddPurchaseWindow(product) { Owner = ParentWindow };
            if (addPurchaseWindow.ShowDialog() == true)
            {
                _currentView.Refresh();
            }
        }

        private void ProductSearchControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentView.Refresh();
        }

        private void ProductCollection_Filter(object sender, FilterEventArgs e)
        {
            Product p = e.Item as Product;
            if (p == null || searchBarcodeInfoTextBox == null || searchBrandNameInfoTextBox == null ||
                searchProductNameInfoTextBox == null)
            {
                e.Accepted = true;
                return;
            }

            string barcode = searchBarcodeInfoTextBox.Text.ToUpperInvariant();
            string brand = searchBrandNameInfoTextBox.Text.ToUpperInvariant();
            string name = searchProductNameInfoTextBox.Text.ToUpperInvariant();

            string productBarcode = p.Barcode == null ? string.Empty : p.Barcode.ToUpperInvariant();
            string productBrand = p.Brand == null ? string.Empty : p.Brand.ToUpperInvariant();
            string productName = p.Name == null ? string.Empty : p.Name.ToUpperInvariant();

            if (productBarcode.Contains(barcode) && productBrand.Contains(brand) &&
                productName.Contains(name))
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
        }

        private void ProductsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is DataGridCell && e.Key == Key.Delete)
            {
                DeleteSelectedProduct();
                e.Handled = true;
            }
        }

        private void DeleteSelectedProduct()
        {
            Product currProduct = productsDataGrid.SelectedItem as Product;
            if (currProduct == null)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show("Seçili ürünü silmek istediğinizden emin misiniz?",
                "Ürün silme onayı", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            if (currProduct.ProductPurchases.Count > 0 || currProduct.SaleItems.Count > 0)
            {
                MessageBox.Show(
                    "Sistemde bu ürün ile ilgili alım satım bilgileri bulunmakta. Ürünü silmek için önce bu " +
                    "bilgileri silmeniz gerekmektedir.\n\nÜrün alış ve satışlarına sırasıyla\n\n" + 
                    "Ekstra -> Geçmiş alış görüntüleme ve \nEkstra -> Geçmiş Satış Görüntüleme\n\n" + 
                    "menülerinden ulaşabilirsiniz.", "Ürün silme", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Products.Remove(currProduct);
            List<ExpirationDate> tmpExDates = new List<ExpirationDate>(currProduct.ExpirationDates);
            foreach (ExpirationDate expirationDate in tmpExDates)
            {
                ObjectCtx.Context.ExpirationDates.Remove(expirationDate);
            }
            ObjectCtx.Context.Products.Remove(currProduct);
            ObjectCtx.Context.SaveChanges();
        }

        private void DatagridDeleteProductMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedProduct();
        }

        private void DatagridAddPurchaseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Product currProduct = productsDataGrid.SelectedItem as Product;
            if (currProduct == null)
            {
                return;
            }
            OpenPurchaseDialog(currProduct.Barcode);
        }

        private void DatagridDetailsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Product product = productsDataGrid.SelectedItem as Product;
            OpenProductDetails(product);
        }

        private void ProductsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow selectedRow = e.Source as DataGridRow;
            if (selectedRow == null)
            {
                return;
            }
            Product selectedProduct = selectedRow.Item as Product;
            OpenProductDetails(selectedProduct);
        }

        private void OpenProductDetails(Product product)
        {
            if (product == null)
            {
                return;
            }
            ProductDetailsWindow productDetailsWindow = new ProductDetailsWindow(product) { Owner = ParentWindow };
            if (productDetailsWindow.ShowDialog() == true)
            {
                _currentView.Refresh();
            }
        }
    }
}
