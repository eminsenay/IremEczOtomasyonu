using System;
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
using System.Windows.Shapes;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private Model1Container _dbContext;

        public AddProductWindow(string barcode)
        {
            InitializeComponent();
            barcodeTextBox.Text = barcode;
        }

        private ObjectQuery<Product> GetProductsQuery(Model1Container model1Container)
        {
            // Auto generated code

            ObjectQuery<Product> productsQuery = model1Container.Products.Where(p => p.Barcode == barcodeTextBox.Text) 
                as ObjectQuery<Product>;
            // Returns an ObjectQuery.
            return productsQuery;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            _dbContext = new Model1Container();
            // Load data into Products. You can modify this code as needed.
            CollectionViewSource productsViewSource = ((CollectionViewSource)(FindResource("productsViewSource")));
            ObjectQuery<Product> productsQuery = GetProductsQuery(_dbContext);
            productsViewSource.Source = productsQuery.Execute(MergeOption.AppendOnly);
            // Load data into ProductPurchases. You can modify this code as needed.
            CollectionViewSource productPurchasesViewSource = ((CollectionViewSource)(FindResource("productPurchasesViewSource")));
            ObjectQuery<ProductPurchase> productPurchasesQuery = GetProductPurchasesQuery(_dbContext);
            productPurchasesViewSource.Source = productPurchasesQuery.Execute(MergeOption.AppendOnly);
        }

        private ObjectQuery<ProductPurchase> GetProductPurchasesQuery(Model1Container model1Container)
        {
            // Auto generated code

            ObjectQuery<ProductPurchase> productPurchasesQuery = model1Container.ProductPurchases;
            // To explicitly load data, you may need to add Include methods like below:
            // productPurchasesQuery = productPurchasesQuery.Include("ProductPurchases.Product").
            // For more information, please see http://go.microsoft.com/fwlink/?LinkId=157380
            // Returns an ObjectQuery.
            return productPurchasesQuery;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            string barcode = barcodeTextBox.Text;
            string productName = nameTextBox.Text;
            string brandName = brandTextBox.Text;
            decimal currBuyingPrice = decimal.Parse(currentBuyingPriceTextBox.Text);
            decimal currSellinPrice = decimal.Parse(currentSellingPriceTextBox.Text);

            int numBought = int.Parse(numItemsBoughtTextBox.Text);
            decimal buyingPrice = decimal.Parse(priceTextBox.Text);
            DateTime buyingDate = (purchaseDateDatePicker.SelectedDate != null) ? 
                (DateTime) purchaseDateDatePicker.SelectedDate : DateTime.Now;
            DateTime expirationDate = (expirationDateDatePicker.SelectedDate != null) ?
                (DateTime)expirationDateDatePicker.SelectedDate : DateTime.Now;
            string buyingRemarks = remarksTextBox.Text;

            Product lastProduct = _dbContext.Products.OrderByDescending(o => o.Id).FirstOrDefault();
            long productId = lastProduct != null ? lastProduct.Id + 1 : 1;
            //Customer lastCustomer = _dbContext.Customers.OrderByDescending(o => o.Id).FirstOrDefault();
            Product product = new Product
                        {
                            Id = productId,
                            Barcode = barcode,
                            Name = productName,
                            Brand = brandName,
                            CurrentBuyingPrice = currBuyingPrice,
                            CurrentSellingPrice = currSellinPrice, 
                            NumItems = numBought,
                        };
            _dbContext.AddToProducts(product);

            ProductPurchase lastPurchase = _dbContext.ProductPurchases.OrderByDescending(pu => pu.Id).FirstOrDefault();
            long purchaseId = lastPurchase != null ? lastPurchase.Id + 1 : 1;
            ProductPurchase purchase = new ProductPurchase
                                       {
                                           Id = purchaseId,
                                           ProductId = productId,
                                           NumItems = numBought,
                                           PurchaseDate = buyingDate,
                                           ExpirationDate = expirationDate,
                                           Price = buyingPrice,
                                           Remarks = buyingRemarks,
                                       };
            _dbContext.AddToProductPurchases(purchase);

            _dbContext.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}
