using System;
using System.Collections.Generic;
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

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for UserControlProducts.xaml
    /// </summary>
    public partial class UserControlProducts : UserControl
    {
        private readonly Model1Container _dbContext;
        private ICollectionView CurrentView { get; set; }
        private List<Product> Products { get; set; }

        public UserControlProducts()
        {
            InitializeComponent();

            _dbContext = new Model1Container();

            CollectionViewSource productsViewSource = ((CollectionViewSource)(FindResource("productsViewSource")));
            Products = _dbContext.Products.ToList();
            productsViewSource.Source = Products;
            CurrentView = productsViewSource.View;
        }

        private void AddNewProduct_Click(object sender, RoutedEventArgs e)
        {
            AddNewProductWindow addProductWindow = new AddNewProductWindow()
                                                {
                                                    Owner = Parent as Window,
                                                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                                                };
            if (addProductWindow.ShowDialog() == true)
            {
                // A product is added. Refresh the datagrid
                Products.Add(addProductWindow.CurrentProduct);
                CurrentView.Refresh();
            }

        }

        private void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
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
            Product product = Products.First(p => p.Barcode == barcodeWindow.Barcode);
            AddPurchaseWindow addPurchaseWindow = new AddPurchaseWindow(product, _dbContext)
            {
                Owner = Parent as Window,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (addPurchaseWindow.ShowDialog() == true)
            {
                // A product is refreshed (number of items, and buying price). Refresh the datagrid
                CurrentView.Refresh();
            }
        }
    }
}
