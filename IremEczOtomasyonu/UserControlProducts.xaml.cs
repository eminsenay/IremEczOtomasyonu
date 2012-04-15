using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Objects;
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
    /// Interaction logic for UserControlProducts.xaml
    /// </summary>
    public partial class UserControlProducts : UserControl
    {
        private ICollectionView _currentView;
        public ObservableCollection<Product> Products { get; private set; }

        public UserControlProducts()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource productsViewSource = ((CollectionViewSource)(FindResource("productsViewSource")));
            Products = new ObservableCollection<Product>(
                ObjectCtx.Context.Products.Include("ProductPurchases").Include("SaleItems"));

            productsViewSource.Source = Products;

            _currentView = productsViewSource.View;
        }

        public void Reload()
        {
            Products.Clear();
            foreach (Product product in ObjectCtx.Context.Products.Include("ProductPurchases").Include("SaleItems"))
            {
                Products.Add(product);
            }
        }

        private void AddNewProduct_Click(object sender, RoutedEventArgs e)
        {
            AddNewProductWindow addProductWindow = new AddNewProductWindow
                                                {
                                                    Owner = Parent as Window,
                                                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                                                };
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
                BarcodeWindow barcodeWindow = new BarcodeWindow
                {
                    Owner = Parent as Window,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
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
            AddPurchaseWindow addPurchaseWindow = new AddPurchaseWindow(product)
                                                  {
                                                      Owner = Parent as Window,
                                                      WindowStartupLocation = WindowStartupLocation.CenterOwner
                                                  };
            addPurchaseWindow.ShowDialog();
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
            if (e.Key == Key.Delete)
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
                result = MessageBox.Show(
                    "Sistemde bu ürün ile ilgili alım satım bilgileri bulunmakta. Ürünü silmeniz bunları " +
                    "kaybetmenize neden olacaktır.\nDevam etmek istiyor musunuz?", 
                    "Ürün silme onayı", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            Products.Remove(currProduct);
            ObjectCtx.Context.DeleteObject(currProduct);
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
            ProductDetailsWindow productDetailsWindow = new ProductDetailsWindow(product)
            {
                Owner = Parent as Window,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (productDetailsWindow.ShowDialog() == true)
            {
               _currentView.Refresh(); 
            }
        }
    }
}
