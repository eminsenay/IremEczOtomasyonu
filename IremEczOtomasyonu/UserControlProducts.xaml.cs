﻿using System;
using System.Collections.Generic;
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
        private readonly CollectionViewSource _productsViewSource;
        //private List<Product> Products { get; set; }
        private ProductCollection Products { get; set; }

        public UserControlProducts()
        {
            InitializeComponent();

            _dbContext = new Model1Container();

            _productsViewSource = ((CollectionViewSource)(FindResource("productsViewSource")));
            ObjectQuery<Product> productsQuery = GetProductsQuery(_dbContext);
            //Products = productsQuery.Execute(System.Data.Objects.MergeOption.AppendOnly).ToList();
            //_productsViewSource.Source = Products;
            //_productsViewSource.Source = productsQuery.Execute(MergeOption.AppendOnly);
            Products = new ProductCollection(productsQuery.Execute(MergeOption.AppendOnly), _dbContext);
            _productsViewSource.Source = Products;
        }

        private ObjectQuery<Product> GetProductsQuery(Model1Container model1Container)
        {
            // Auto generated code

            ObjectQuery<Product> productsQuery = model1Container.Products;
            // Update the query to include Purchases data in Customers. You can modify this code as needed.
            //customersQuery = customersQuery.Include("ProductSales");
            // Returns an ObjectQuery.
            return productsQuery;
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
                //Products.Add(addProductWindow.CurrentProduct);
                //_productsViewSource.View.Refresh();
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
            AddPurchaseWindow addPurchaseWindow = new AddPurchaseWindow(barcodeWindow.Barcode)
            {
                Owner = Parent as Window,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (addPurchaseWindow.ShowDialog() == true)
            {
                // A product is refreshed (number of items, and buying price). Refresh the datagrid
                _productsViewSource.View.Refresh();
            }
        }
    }
}
