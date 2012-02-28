using System;
using System.Collections.Generic;
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
        private List<Product> Products { get; set; }

        public UserControlProducts()
        {
            InitializeComponent();

            _dbContext = new Model1Container();

            _productsViewSource = ((CollectionViewSource)(FindResource("productsViewSource")));
            System.Data.Objects.ObjectQuery<Product> productsQuery = GetProductsQuery(_dbContext);
            Products = productsQuery.Execute(System.Data.Objects.MergeOption.AppendOnly).ToList();
            _productsViewSource.Source = Products;
        }

        private System.Data.Objects.ObjectQuery<Product> GetProductsQuery(Model1Container model1Container)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<Product> productsQuery = model1Container.Products;
            // Update the query to include Purchases data in Customers. You can modify this code as needed.
            //customersQuery = customersQuery.Include("ProductSales");
            // Returns an ObjectQuery.
            return productsQuery;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            BarcodeWindow barcodeWindow = new BarcodeWindow
                                          {
                                              Owner = Parent as Window, 
                                              WindowStartupLocation = WindowStartupLocation.CenterOwner
                                          };
            bool? res = barcodeWindow.ShowDialog();
            string barcode = barcodeWindow.Barcode;
        }
    }
}
